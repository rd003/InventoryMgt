import { computed, DestroyRef, inject, Injectable, signal } from "@angular/core";
import { SupplierModel } from "./supplier.model";
import { HttpErrorResponse } from "@angular/common/http";
import { SupplierService } from "./supplier.service";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";

export interface SupplierState {
    suppliers: readonly SupplierModel[],
    searchTerm: string | null;
    sortColumn: string | null;
    sortDirection: string | null;
    page: number;
    limit: number;
    totalRecords: number;
    loading: boolean,
    error: HttpErrorResponse | null
}

@Injectable({ providedIn: "root" })
export class SupplierStore {
    private supplierService = inject(SupplierService);
    private destroyRef = inject(DestroyRef);

    private initialState: SupplierState = {
        suppliers: [],
        searchTerm: null,
        sortColumn: null,
        sortDirection: null,
        page: 1,
        limit: 4,
        totalRecords: 0, loading: false,
        error: null
    };

    private store = signal(this.initialState);

    suppliers = computed(() => this.store().suppliers);
    loading = computed(() => this.store().loading);
    error = computed(() => this.store().error);
    searchTerm = computed(() => this.store().searchTerm);
    sortColumn = computed(() => this.store().sortColumn);
    sortDirection = computed(() => this.store().sortDirection);
    page = computed(() => this.store().page);
    limit = computed(() => this.store().limit);

    addSupplier = (supplier: SupplierModel) => {
        this.setLoading(true);
        this.supplierService.addSupplier(supplier).pipe(
            takeUntilDestroyed(this.destroyRef)
        ).subscribe({
            next: ((createdSupplier) => {
                this.store.update((prevState) => ({
                    ...prevState,
                    loading: false,
                    suppliers: [...prevState.suppliers, createdSupplier]
                }));
            }),
            error: (error => this.setError(error))
        });
    }

    private loadSuppliers = () => {
        this.setLoading(true);
        this.supplierService.getSuppliers().pipe(
            takeUntilDestroyed(this.destroyRef)
        ).subscribe(
            {
                next: (data) => {
                    this.store.update((prevState) => ({
                        ...prevState,
                        suppliers: data.suppliers,
                        loading: false
                    }))
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