export interface EffectiveDateSummary {
  id: string;
  month: number;
  year: number;
  display: string;
  balance: number;
  payableCount: number;
  receivableCount: number;
}

export interface EffectiveDateView {
  receivable: ReceivableView[];
  payable: any[]; // TODO
  cardExpense: CardExpenseView[];
}

export interface ImmediateExpenseView {
  // TODO: Define properties
}

export interface CardExpenseView {
  id: string;
}

export interface CardExpenseItemView {
  // TODO: Define properties
}

export interface ReceivableView {
  id: string;
}