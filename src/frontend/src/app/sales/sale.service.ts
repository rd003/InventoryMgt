import { Injectable, inject } from "@angular/core";
import { environment } from "../../environments/environment.development";
import { HttpClient, HttpParams } from "@angular/common/http";
import { PaginatedSale, SaleModel } from "./sale.model";
import { Observable, delay, map } from "rxjs";
import { PaginationModel } from "../shared/models/pagination.model";
import { SaleFilterParams } from "./sale-filter-params.model";

@Injectable({ providedIn: "root" })
export class SaleService {
  private readonly baseUrl = environment.API_BASE_URL + "/sales";
  private readonly http = inject(HttpClient);

  addSale(sale: SaleModel): Observable<SaleModel> {
    return this.http.post<SaleModel>(this.baseUrl, sale);
  }

  updateSale(sale: SaleModel): Observable<SaleModel> {
    const url = `${this.baseUrl}/${sale.id}`;
    return this.http.put<SaleModel>(url, sale);
  }

  deleteSale(id: number): Observable<any> {
    const url = `${this.baseUrl}/${id}`;
    return this.http.delete<any>(url);
  }

  getSaleById(id: string): Observable<SaleModel> {
    const url = `${this.baseUrl}/${id}`;
    return this.http.get<SaleModel>(url);
  }

  getSales(
    page = 1,
    limit = 4,
    productName: string | null = null,
    dateFrom: string | null = null,
    dateTo: string | null = null,
    sortColumn = "Id",
    sortDirection: "asc" | "desc" = "asc"
  ): Observable<PaginatedSale> {
    const params = this.buildQueryParams({ productName, dateFrom, dateTo, sortColumn, sortDirection, page, limit });

    return this.http
      .get(this.baseUrl, {
        params: params,
        observe: "response",
      })
      .pipe(
        map((response) => {
          const pagination: PaginationModel = JSON.parse(
            response.headers.get("X-Pagination") as string
          );
          const sales = response.body as SaleModel[];
          const paginatedSale: PaginatedSale = { ...pagination, sales };
          return paginatedSale;
        })
      )
    //
  }

  downloadPdf(
    productName: string | null = null,
    dateFrom: string | null = null,
    dateTo: string | null = null,
    sortColumn = "Id",
    sortDirection: "asc" | "desc" = "asc"): Observable<Blob> {
    const parameters = this.buildQueryParams({
      productName,
      dateFrom,
      dateTo,
      sortColumn,
      sortDirection,
    });
    return this.http.get(this.baseUrl, {
      params: parameters,
      responseType: 'blob',
      headers: {
        'Accept': 'application/pdf'
      }
    });
  }

  private buildQueryParams(filters: SaleFilterParams): HttpParams {
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
