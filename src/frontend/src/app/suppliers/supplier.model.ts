import { PaginationModel } from "../shared/models/pagination.model";

export interface SupplierModel {
    id: number;
    supplierName: string;
    contactPerson: string;
    email: string;
    phone: string;
    address: string;
    city: string;
    state: string;
    country: string;
    postalCode: string;
    taxNumber: string;
    paymentTerms: number;
    isActive: boolean;
}

export interface PaginatedSupplier extends PaginationModel {
    suppliers: SupplierModel[];
}