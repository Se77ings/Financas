import CardExpenseItemSummary from "./Item/CardExpenseItemSummary";
import { Deserializer } from "@/services/Deserializer";

export default class CardExpenseSummary {
    id: string;
    description: string;
    total: number;
    dueDate: string;
    subtotal: number;
    discounts: number;
    observation: string;
    creditCardId: string;
    items: CardExpenseItemSummary[];
    firstItemDate: string | null;
    lastItemDate: string | null;

    constructor(id: string, description: string, total: number, dueDate: string, subtotal: number, discounts: number, observation:string, creditCardId: string, items: CardExpenseItemSummary[], firstItemDate: string | null = null, lastItemDate: string | null = null) {
        this.id = id;
        this.description = description;
        this.total = total;
        this.dueDate = dueDate;
        this.subtotal = subtotal;
        this.discounts = discounts;
        this.observation = observation;
        this.creditCardId = creditCardId;
        this.items = items;
        this.firstItemDate = firstItemDate;
        this.lastItemDate = lastItemDate;
    }

    static fromJSON(data: any): CardExpenseSummary {
        return new CardExpenseSummary(
            data.id,
            data.description,
            data.total,
            data.dueDate,
            data.subtotal || data.subTotal,
            data.discounts,
            data.observation,
            data.creditCardId || data.creditCardId,
            Deserializer.deserializeArray(data.items || [], CardExpenseItemSummary),
            data.firstItemDate ?? null,
            data.lastItemDate ?? null
        );
    }
}
