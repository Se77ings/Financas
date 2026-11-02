import type { FrequencyType } from "@/services/recurrentPaymentServices";

export default class RecurrentPaymentView {
    id: string;
    description: string;
    value: number;
    frequencyType: FrequencyType;
    creditCardId: string | null;
    creditCardName: string | null;
    startDate: string;
    endDate: string | null;
    isActive: boolean;
    categoryId: string | null;

    constructor(id: string, description: string, value: number, frequencyType: FrequencyType, creditCardId: string | null, creditCardName: string | null, startDate: string, endDate: string | null, isActive: boolean, categoryId: string | null) {
        this.id = id;
        this.description = description;
        this.value = value;
        this.frequencyType = frequencyType;
        this.creditCardId = creditCardId;
        this.creditCardName = creditCardName;
        this.startDate = startDate;
        this.endDate = endDate;
        this.isActive = isActive;
        this.categoryId = categoryId;
    }

    static fromJSON(data: any): RecurrentPaymentView {
        return new RecurrentPaymentView(
            data.id,
            data.description,
            data.value,
            data.frequencyType,
            data.creditCardId ?? null,
            data.creditCardName ?? null,
            data.startDate,
            data.endDate ?? null,
            data.isActive,
            data.categoryId ?? null
        );
    }
}
