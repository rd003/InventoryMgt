import { Injectable, inject } from "@angular/core";
import { PaginatedPurchase, PurchaseModel } from "./purchase.model";
import { HttpClient, HttpParams } from "@angular/common/http";
import { environment } from "../../environments/environment.development";
import { Observable, map } from "rxjs";
import { PaginationModel } from "../shared/models/pagination.model";
import { PurchaseFilterParams } from "./purchase-filter-params.model";

@Injectable({
  providedIn: "root",
})
export class PurchaseService {
  private readonly _http = inject(HttpClient);
  private baseUrl = environment.API_BASE_URL + "/purchases";

  add(purchase: PurchaseModel): Observable<PurchaseModel> {
    return this._http.post<PurchaseModel>(this.baseUrl, purchase);
  }

  update(purchase: PurchaseModel): Observable<PurchaseModel> {
    const url = `${this.baseUrl}/${purchase.id}`;
    return this._http.put<PurchaseModel>(url, purchase);
  }

  delete(id: number): Observable<any> {
    const url = `${this.baseUrl}/${id}`;
    return this._http.delete<any>(url);
  }

  getById(id: number): Observable<PurchaseModel> {
    const url = `${this.baseUrl}/${id}`;
    return this._http.get<PurchaseModel>(url);
  }

  getAll(
    page = 1,
    limit = 4,
    productName: string | null = null,
    dateFrom: string | null = null,
    dateTo: string | null = null,
    sortColumn = "Id",
    sortDirection: "asc" | "desc" = "asc"
  ): Observable<PaginatedPurchase> {
    const parameters = this.buildQueryParams({
      page,
      limit,
      productName,
      dateFrom,
      dateTo,
      sortColumn,
      sortDirection,
    });
    return this._http
      .get(this.baseUrl, {
        observe: "response",
        params: parameters,
      })
      .pipe(
        map((response) => {
          const paginationJson = response.headers.get("X-Pagination") as string;
          const pagination = JSON.parse(paginationJson) as PaginationModel;
          const purchases = response.body as PurchaseModel[];
          const data: PaginatedPurchase = { ...pagination, purchases };
          // console.log(response);
          return data;
        })
      );
  }

  /**
   * Downloads a PDF report of purchases with applied filters
   * @returns Observable<Blob> - PDF file as blob
   */
  downloadPdf(
    productName: string | null = null,
    dateFrom: string | null = null,
    dateTo: string | null = null,
    sortColumn = "Id",
    sortDirection: "asc" | "desc" = "asc"
  ): Observable<Blob> {
    const parameters = this.buildQueryParams({
      productName,
      dateFrom,
      dateTo,
      sortColumn,
      sortDirection,
    });
    // console.log(parameters);
    return this._http.get(this.baseUrl, {
      params: parameters,
      responseType: "blob",
      headers: {
        Accept: "application/pdf",
      },
    });
  }

  /**
  * Builds common HTTP parameters for purchase queries
  */
  private buildQueryParams(filters: PurchaseFilterParams): HttpParams {
    let parameters = new HttpParams();

    if (filters.sortColumn) {
      parameters = parameters.set("sortColumn", filters.sortColumn);
    }
    if (filters.sortDirection) {
      parameters = parameters.set("sortDirection", filters.sortDirection);
    }
    if (filters.page !== undefined) {
      parameters = parameters.set("page", filters.page);
    }
    if (filters.limit !== undefined) {
      parameters = parameters.set("limit", filters.limit);
    }
    if (filters.productName) {
      parameters = parameters.set("productName", filters.productName);
    }
    if (filters.dateFrom && filters.dateTo) {
      parameters = parameters.set("dateFrom", filters.dateFrom);
      parameters = parameters.set("dateTo", filters.dateTo);
    }

    return parameters;
  }

}
