import {
  MAT_SELECT_CONFIG,
  MAT_SELECT_SCROLL_STRATEGY,
  MAT_SELECT_SCROLL_STRATEGY_PROVIDER,
  MAT_SELECT_SCROLL_STRATEGY_PROVIDER_FACTORY,
  MAT_SELECT_TRIGGER,
  MatSelect,
  MatSelectChange,
  MatSelectModule,
  MatSelectTrigger
} from "./chunk-K6TXVGHZ.js";
import "./chunk-LOA7KBO5.js";
import "./chunk-OPJPP4X5.js";
import "./chunk-42GBDDDK.js";
import "./chunk-ZA74PSUP.js";
import {
  MatError,
  MatFormField,
  MatHint,
  MatLabel,
  MatPrefix,
  MatSuffix
} from "./chunk-UHNRCFOB.js";
import "./chunk-YUQWKBJ5.js";
import {
  MatOptgroup,
  MatOption
} from "./chunk-YV6G27HH.js";
import "./chunk-EIEJ2MWA.js";
import "./chunk-7KQVYRAX.js";
import "./chunk-JAB54QQG.js";
import "./chunk-CWDR7GXC.js";
import "./chunk-UCOKQCTQ.js";
import "./chunk-DQ7OVFPD.js";
import "./chunk-QCETVJKM.js";
import "./chunk-U5GGKGSK.js";
import "./chunk-EOFW2REK.js";
import "./chunk-JOK47D7D.js";
import "./chunk-LJALDTNB.js";
import "./chunk-BNZZXCEE.js";
import "./chunk-HTKYBOPB.js";
import "./chunk-ZH6LALFQ.js";
import "./chunk-FXLWFISP.js";
import "./chunk-SKP6K72R.js";
import "./chunk-LZU6IAWD.js";
import "./chunk-EJGFBIB2.js";
import "./chunk-WDMUDEB6.js";

// node_modules/@angular/material/fesm2022/select.mjs
var matSelectAnimations = {
  // Represents
  // trigger('transformPanel', [
  //   state(
  //     'void',
  //     style({
  //       opacity: 0,
  //       transform: 'scale(1, 0.8)',
  //     }),
  //   ),
  //   transition(
  //     'void => showing',
  //     animate(
  //       '120ms cubic-bezier(0, 0, 0.2, 1)',
  //       style({
  //         opacity: 1,
  //         transform: 'scale(1, 1)',
  //       }),
  //     ),
  //   ),
  //   transition('* => void', animate('100ms linear', style({opacity: 0}))),
  // ])
  /** This animation transforms the select's overlay panel on and off the page. */
  transformPanel: {
    type: 7,
    name: "transformPanel",
    definitions: [
      {
        type: 0,
        name: "void",
        styles: {
          type: 6,
          styles: { opacity: 0, transform: "scale(1, 0.8)" },
          offset: null
        }
      },
      {
        type: 1,
        expr: "void => showing",
        animation: {
          type: 4,
          styles: {
            type: 6,
            styles: { opacity: 1, transform: "scale(1, 1)" },
            offset: null
          },
          timings: "120ms cubic-bezier(0, 0, 0.2, 1)"
        },
        options: null
      },
      {
        type: 1,
        expr: "* => void",
        animation: {
          type: 4,
          styles: { type: 6, styles: { opacity: 0 }, offset: null },
          timings: "100ms linear"
        },
        options: null
      }
    ],
    options: {}
  }
};
export {
  MAT_SELECT_CONFIG,
  MAT_SELECT_SCROLL_STRATEGY,
  MAT_SELECT_SCROLL_STRATEGY_PROVIDER,
  MAT_SELECT_SCROLL_STRATEGY_PROVIDER_FACTORY,
  MAT_SELECT_TRIGGER,
  MatError,
  MatFormField,
  MatHint,
  MatLabel,
  MatOptgroup,
  MatOption,
  MatPrefix,
  MatSelect,
  MatSelectChange,
  MatSelectModule,
  MatSelectTrigger,
  MatSuffix,
  matSelectAnimations
};
//# sourceMappingURL=@angular_material_select.js.map
