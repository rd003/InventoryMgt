import { HttpClient } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../environments/environment.development";
import { Observable, of } from "rxjs";
import { PaginatedSupplier, SupplierModel } from "./supplier.model";

@Injectable({ providedIn: 'root' })
export class SupplierService {
    private readonly _http = inject(HttpClient);
    private _url = environment.API_BASE_URL + "/suppliers";

    getSuppliers = (page = 1,
        limit = 4,
        searchTerm: string | null = null,
        sortColumn: string | null = null,
        sortDirection: string | null = null): Observable<PaginatedSupplier> => {
        //this._http.get(this._url);

        const suppliers: SupplierModel[] = [
            {
                "id": 1,
                "supplierName": "Supplier 2",
                "contactPerson": "Satyendra Singh",
                "email": "satyendrasingh@example.com",
                "phone": "+91-1111-123-456",
                "address": "312 Some Ave, Suite",
                "city": "Haridwar",
                "state": "Uttarakhand",
                "country": "India",
                "postalCode": "100021",
                "taxNumber": "TAX-544456789",
                "paymentTerms": 40,
                "isActive": true
            }
        ];
        const paginatedSupplier: PaginatedSupplier = {
            suppliers,
            Limit: 5,
            Page: 1,
            TotalPages: 10,
            TotalRecords: 50
        };
        return of(paginatedSupplier);
    }


}