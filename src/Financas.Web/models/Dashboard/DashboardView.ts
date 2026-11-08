export interface DashboardTotals {
    income: number;
    expenses: number;
    balance: number;
}

/** `creditCardName` nulo indica gasto à vista; `installment` vem como "2/3" só nos parcelados. */
export interface SpendingEntry {
    description: string;
    date: string;
    total: number;
    categoryId: string | null;
    creditCardName: string | null;
    installment: string | null;
}

export interface DailySpendingDay {
    date: string;
    entries: SpendingEntry[];
    idealDaily: number | null;
}

export interface DailySpending {
    lastDays: DailySpendingDay[];
    selectedMonth: DailySpendingDay[];
}

export interface SpendingPaceDay {
    day: number;
    accumulated: number | null;
    previousMonthAccumulated: number | null;
    idealAccumulated: number;
}

export interface SpendingPace {
    idealDaily: number;
    days: SpendingPaceDay[];
}

export interface CategorySpending {
    categoryId: string | null;
    name: string;
    total: number;
    percentage: number;
    entries: SpendingEntry[];
}

export interface CreditCardInvoiceAmount {
    creditCardId: string;
    name: string;
    total: number;
}

/** Uma competência, ou um ano inteiro quando `month` e `effectiveDateId` vêm nulos; `invoices` traz só os cartões com fatura. */
export interface CreditCardInvoicePeriod {
    effectiveDateId: string | null;
    month: number | null;
    year: number;
    isSelected: boolean;
    invoices: CreditCardInvoiceAmount[];
}

export interface InstallmentCommitment {
    creditCardName: string;
    total: number;
}

export interface FutureCommitmentMonth {
    month: number;
    year: number;
    installmentsByCreditCard: InstallmentCommitment[];
    cardRecurrentPayments: number;
    immediateRecurrentPayments: number;
    total: number;
}

export interface FutureCommitments {
    averageIncome: number;
    months: FutureCommitmentMonth[];
}

/** Uma competência, ou um ano inteiro quando `month` e `effectiveDateId` vêm nulos. */
export interface HistoryPeriod {
    effectiveDateId: string | null;
    month: number | null;
    year: number;
    income: number;
    expenses: number;
    balance: number;
}

/** Em todos os anos, os quadros de um único mês vêm nulos e os históricos trazem uma linha por ano. */
export default interface DashboardView {
    totals: DashboardTotals;
    dailySpending: DailySpending | null;
    spendingPace: SpendingPace | null;
    spendingByCategory: CategorySpending[];
    creditCardHistory: CreditCardInvoicePeriod[];
    futureCommitments: FutureCommitments | null;
    history: HistoryPeriod[];
}
