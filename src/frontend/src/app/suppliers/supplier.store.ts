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

@Injectable()
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


    updateSupplier = (supplier: SupplierModel) => {
        this.setLoading(true);
        this.supplierService.updateSupplier(supplier).pipe(
            takeUntilDestroyed(this.destroyRef)
        ).subscribe({
            next: ((_) => {
                this.store.update((prevState) => ({
                    ...prevState,
                    loading: false,
                    suppliers: prevState.suppliers.map(s => s.id === supplier.id ? supplier : s)
                }));
            }),
            error: (error => this.setError(error))
        });
    }

    deleteSupplier = (supplierId: number) => {
        this.setLoading(true);
        this.supplierService.deleteSupplier(supplierId).pipe(
            takeUntilDestroyed(this.destroyRef)
        ).subscribe({
            next: ((_) => {
                this.store.update((prevState) => ({
                    ...prevState,
                    loading: false,
                    suppliers: prevState.suppliers.filter(s => s.id !== supplierId)
                }));
            }),
            error: (error => this.setError(error))
        });
    }

    private getCurrentState = () => {
        const currentState = this.store(); // or however you access your current state
        return {
            page: currentState.page || 1,
            limit: currentState.limit || 4,
            searchTerm: currentState.searchTerm || null,
            sortColumn: currentState.sortColumn || null,
            sortDirection: currentState.sortDirection || null
        };
    };

    setPage = (page: number) => {
        const current = this.getCurrentState();
        this.loadSuppliers(page, current.limit, current.searchTerm, current.sortColumn, current.sortDirection);
    }

    setLimit = (limit: number) => {
        const current = this.getCurrentState();
        this.loadSuppliers(current.page, limit, current.searchTerm, current.sortColumn, current.sortDirection);
    };

    setSearch = (searchTerm: string) => {
        const current = this.getCurrentState();
        // Reset to page 1 when searching (common UX pattern)
        this.loadSuppliers(1, current.limit, searchTerm, current.sortColumn, current.sortDirection);
    };

    setSortColumn = (sortColumn: string) => {
        const current = this.getCurrentState();
        this.loadSuppliers(current.page, current.limit, current.searchTerm, sortColumn, current.sortDirection);
    };

    setSortDirection = (sortDirection: string) => {
        const current = this.getCurrentState();
        this.loadSuppliers(current.page, current.limit, current.searchTerm, current.sortColumn, sortDirection);
    };

    private loadSuppliers = (page = 1,
        limit = 4,
        searchTerm: string | null = null,
        sortColumn: string | null = null,
        sortDirection: string | null = null) => {
        this.setLoading(true);
        this.supplierService.getSuppliers(page, limit, searchTerm, sortColumn, sortDirection).pipe(
            takeUntilDestroyed(this.destroyRef)
        ).subscribe(
            {
                next: (data) => {
                    this.store.update((prevState) => ({
                        ...prevState,
                        suppliers: data.suppliers,
                        loading: false,
                        page,
                        limit,
                        searchTerm,
                        sortColumn,
                        sortDirection,
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