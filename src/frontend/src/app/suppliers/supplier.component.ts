import { ChangeDetectionStrategy, Component, inject } from "@angular/core";
import { SupplierStore } from "./supplier.store";
import { MatButtonModule } from "@angular/material/button";
import { MatProgressSpinnerModule } from "@angular/material/progress-spinner";

@Component({
    selector: 'app-supplier',
    imports: [
        MatProgressSpinnerModule,
        MatButtonModule,
    ],
    providers: [SupplierStore],
    template: `
       <div style="display: flex;align-items:center;gap:5px;margin-bottom:8px">
        <span style="font-size: 26px;font-weight:bold"> Suppliers </span>
        <button style="margin:10px 0px"
        mat-raised-button
        color="primary"
        (click)="onAddUpdate('Add Product')"
      >
        Add More
      </button>
      </div>

      @if(store.loading()){
        <div class="spinner-center">
          <mat-spinner diameter="50"></mat-spinner>
        </div>
      }

     @if(store.suppliers() && store.suppliers().length > 0){
         @for (supplier of store.suppliers(); track supplier.id) {
            <p>{{supplier.id}}</p>
         }
     }
     @else {
         <p style="margin-top:20px;font-size:21px">
          No records found
        </p>
     }
    `,
    styles: [``],
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SupplierComponent {
    store = inject(SupplierStore);

    onAddUpdate = (title: string) => {

    }
}