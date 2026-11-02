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

export interface CreditCardUsage {
    creditCardId: string;
    name: string;
    limit: number;
    closingDay: number;
    dueDay: number;
    invoiceTotal: number;
    invoiceDueDate: string | null;
    futureInstallments: number;
    committed: number;
    usagePercentage: number;
}

export interface CreditCardInvoiceAmount {
    creditCardId: string;
    name: string;
    total: number;
}

/** `invoices` traz só os cartões com fatura na competência. */
export interface CreditCardInvoiceMonth {
    effectiveDateId: string;
    month: number;
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

export interface MonthlyHistory {
    effectiveDateId: string;
    month: number;
    year: number;
    income: number;
    expenses: number;
    balance: number;
}

export default interface DashboardView {
    totals: DashboardTotals;
    dailySpending: DailySpending;
    spendingPace: SpendingPace;
    spendingByCategory: CategorySpending[];
    creditCards: CreditCardUsage[];
    creditCardHistory: CreditCardInvoiceMonth[];
    futureCommitments: FutureCommitments;
    monthlyHistory: MonthlyHistory[];
}
