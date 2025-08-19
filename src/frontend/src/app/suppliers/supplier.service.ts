import { HttpClient, HttpParams } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { environment } from "../../environments/environment.development";
import { map, Observable, of } from "rxjs";
import { PaginatedSupplier, SupplierModel } from "./supplier.model";
import { PaginationModel } from "../shared/models/pagination.model";

@Injectable({ providedIn: 'root' })
export class SupplierService {
    private readonly _http = inject(HttpClient);
    private _url = environment.API_BASE_URL + "/suppliers";

    addSupplier = (supplier: SupplierModel) => this._http.post<SupplierModel>(this._url, supplier);

    updateSupplier = (supplier: SupplierModel) => this._http.put<void>(`${this._url}/${supplier.id}`, supplier)

    getSupplier = (supplierId: number) => this._http.get<SupplierModel>(`${this._url}/${supplierId}`);

    deleteSupplier = (supplierId: number) => this._http.delete<SupplierModel>(`${this._url}/${supplierId}`);

    getSuppliers = (page = 1,
        limit = 4,
        searchTerm: string | null = null,
        sortColumn: string | null = null,
        sortDirection: string | null = null): Observable<PaginatedSupplier> => {
        //this._http.get(this._url);

        let parameters = new HttpParams();
        parameters = parameters.set("page", page);
        parameters = parameters.set("limit", limit);
        if (searchTerm)
            parameters = parameters.set("searchTerm", searchTerm);
        if (sortColumn)
            parameters = parameters.set("sortColumn", sortColumn);
        if (sortDirection)
            parameters = parameters.set("sortDirection", sortDirection);
        return this._http.get(this._url, {
            observe: 'response',
            params: parameters
        }).pipe(
            map((response) => {
                const paginationHeader = response.headers.get("X-Pagination") as string;  // it is a json string
                const paginationData: PaginationModel = JSON.parse(paginationHeader);
                const suppliers = response.body as SupplierModel[];
                const supplierResponse: PaginatedSupplier = {
                    ...paginationData, suppliers
                }
                return supplierResponse;
            })
        );
    }


}