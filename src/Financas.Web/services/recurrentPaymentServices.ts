import { getAsync, postAsync, putAsync, deleteAsync, readErrorAsync } from "./baseServices";
import RecurrentPaymentView from "@/models/RecurrentPayment/RecurrentPaymentView";
import { Deserializer } from "./Deserializer";

export enum FrequencyType {
    Mensal = 1,
    Trimestral = 2,
    Semestral = 3,
    Anual = 4
}

export interface RecurrentPaymentRequest {
    description: string;
    value: number;
    frequencyType: FrequencyType;
    creditCardId?: string | null;
    startDate: string;
    endDate?: string | null;
    isActive: boolean;
    categoryId: string | null;
}

export const FREQUENCY_LABELS: Record<FrequencyType, string> = {
    [FrequencyType.Mensal]: 'Mensal',
    [FrequencyType.Trimestral]: 'Trimestral',
    [FrequencyType.Semestral]: 'Semestral',
    [FrequencyType.Anual]: 'Anual',
};

export async function addRecurrentPaymentAsync(request: RecurrentPaymentRequest): Promise<void> {
    const response = await postAsync(`api/RecurrentPayment`, request);
    
    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to add recurrentPayment'));
    }
}

export async function getRecurrentPaymentsAsync(): Promise<any[]> {
    const response = await getAsync(`api/RecurrentPayment`);
    
    if (!response.ok) {
        throw new Error(`Failed to fetch recurrentPayments: ${response.statusText}`);
    }

    return await response.json();
}

export async function getRecurrentPaymentsByCreditCardAsync(creditCardId: string): Promise<RecurrentPaymentView[]> {
    const response = await getAsync(`api/RecurrentPayment/credit-card/${creditCardId}`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to fetch recurrentPayments'));
    }

    const data = await response.json();
    return Deserializer.deserializeArray(data, RecurrentPaymentView);
}

export async function getImmediateExpenseRecurrentPaymentsAsync(): Promise<RecurrentPaymentView[]> {
    const response = await getAsync(`api/RecurrentPayment/immediate-expense`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to fetch immediate expense recurrentPayments'));
    }

    const data = await response.json();
    return Deserializer.deserializeArray(data, RecurrentPaymentView);
}

export async function updateRecurrentPaymentAsync(id: string, request: RecurrentPaymentRequest): Promise<void> {
    const response = await putAsync(`api/RecurrentPayment/${id}`, request);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to update recurrentPayment'));
    }
}

export async function deleteRecurrentPaymentAsync(id: string): Promise<void> {
    const response = await deleteAsync(`api/RecurrentPayment/${id}`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to delete recurrentPayment'));
    }
}
