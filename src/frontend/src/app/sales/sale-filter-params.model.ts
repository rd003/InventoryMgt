export interface SaleFilterParams {
    productName?: string | null;
    dateFrom?: string | null;
    dateTo?: string | null;
    sortColumn?: string;
    sortDirection?: "asc" | "desc";
    page?: number;
    limit?: number;
}