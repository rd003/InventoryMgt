import { computed, DestroyRef, inject, Injectable, signal } from "@angular/core";
import { SupplierModel } from "./supplier.model";
import { HttpErrorResponse } from "@angular/common/http";
import { SupplierService } from "./supplier.service";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";

export interface SupplierState {
    suppliers: readonly SupplierModel[],
    loading: boolean,
    error: HttpErrorResponse | null
}

@Injectable({ providedIn: "root" })
export class SupplierStore {
    private supplierService = inject(SupplierService);
    private destroyRef = inject(DestroyRef);

    private initialState: SupplierState = {
        suppliers: [],
        loading: false,
        error: null
    };

    private store = signal(this.initialState);

    suppliers = computed(() => this.store().suppliers);
    loading = computed(() => this.store().loading);
    error = computed(() => this.store().error);

    private loadSuppliers = () => {
        this.setLoading(true);
        this.supplierService.getSuppliers().pipe(
            takeUntilDestroyed(this.destroyRef)
        ).subscribe(
            {
                next: (data) => {
                    this.store.set({
                        suppliers: data.suppliers,
                        loading: false,
                        error: null
                    })
                },
                error: (err) => {
                    console.log(err);
                    this.setError(err);
                }
            }
        );
    }

    private setLoading = (loading: boolean) => {
        this.store.update((prevState) => ({
            ...prevState,
            loading
        }))
    }

    private setError = (error: HttpErrorResponse) => {
        this.store.update((prevState) => ({
            ...prevState,
            loading: false,
            error
        }));
    }

    constructor() {
        this.loadSuppliers();
    }
}