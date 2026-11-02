export interface DashboardTotals {
    income: number;
    expenses: number;
    balance: number;
}

/** `categoryId` nulo agrupa os lançamentos sem categoria. */
export interface CategoryAmount {
    categoryId: string | null;
    total: number;
}

export interface DailySpendingDay {
    date: string;
    total: number;
    byCategory: CategoryAmount[];
    idealDaily: number | null;
}

export interface DailySpending {
    days: DailySpendingDay[];
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

export interface InstallmentCommitment {
    creditCardName: string;
    total: number;
}

export interface FutureCommitmentMonth {
    month: number;
    year: number;
    installmentsByCreditCard: InstallmentCommitment[];
    cardSubscriptions: number;
    immediateRecurrences: number;
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
    futureCommitments: FutureCommitments;
    monthlyHistory: MonthlyHistory[];
}
