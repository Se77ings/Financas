import { formatCalendarMonthAbbrev } from '@/utils/calendarMonth';
import ReceivableSummaryView from '@/models/Receivable/ReceivableSummary';
import ImmediateExpenseSummary from '@/models/ImmediateExpense/ImmediateExpenseSummary';
import CardExpenseSummary from '@/models/CardExpense/CardExpenseSummary';

export default class EffectiveDateView {
    month: number;
    year: number;
    monthAbbreviation: string;
    periodStart: string;
    periodEnd: string;
    receivables: ReceivableSummaryView[];
    immediateExpenses: ImmediateExpenseSummary[];
    cardExpenses: CardExpenseSummary[];

    constructor(
        id: string,
        month: number,
        year: number,
        periodStart: string,
        periodEnd: string,
        receivables: ReceivableSummaryView[],
        immediateExpenses: ImmediateExpenseSummary[],
        cardExpenses: CardExpenseSummary[]
    ) {
        this.month = month;
        this.year = year;
        this.monthAbbreviation = formatCalendarMonthAbbrev(month);
        this.periodStart = periodStart;
        this.periodEnd = periodEnd;
        this.receivables = receivables;
        this.immediateExpenses = immediateExpenses;
        this.cardExpenses = cardExpenses;
    }

    static fromJSON(data: any): EffectiveDateView {
        const month = typeof data.month === 'number' ? data.month : Number(data.month);
        return new EffectiveDateView(
            data.id ?? '',
            month,
            data.year,
            data.periodStart ?? '',
            data.periodEnd ?? '',
            (data.receivable ?? []).map((r: any) => ReceivableSummaryView.fromJSON(r)),
            (data.payable ?? []).map((p: any) => ImmediateExpenseSummary.fromJSON(p)),
            (data.cardExpense ?? []).map((c: any) => CardExpenseSummary.fromJSON(c))
        );
    }
}
