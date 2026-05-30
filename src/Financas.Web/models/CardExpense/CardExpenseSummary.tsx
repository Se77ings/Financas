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
    

    constructor(id: string, description: string, total: number, dueDate: string, subtotal: number, discounts: number, observation:string, creditCardId: string, items: CardExpenseItemSummary[]) {
        this.id = id;
        this.description = description;
        this.total = total;
        this.dueDate = dueDate;
        this.subtotal = subtotal;
        this.discounts = discounts;
        this.observation = observation;
        this.creditCardId = creditCardId;
        this.items = items;
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
            Deserializer.deserializeArray(data.items || [], CardExpenseItemSummary)
        );
    }
}
