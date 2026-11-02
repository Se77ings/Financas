import { getAsync, postAsync, putAsync, deleteAsync, readErrorAsync } from "./baseServices";
import SubscriptionView from "@/models/Subscription/SubscriptionView";
import { Deserializer } from "./Deserializer";

export enum FrequencyType {
    Mensal = 1,
    Trimestral = 2,
    Semestral = 3,
    Anual = 4
}

export interface SubscriptionRequest {
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

export async function addSubscriptionAsync(request: SubscriptionRequest): Promise<void> {
    const response = await postAsync(`api/Subscription`, request);
    
    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to add subscription'));
    }
}

export async function getSubscriptionsAsync(): Promise<any[]> {
    const response = await getAsync(`api/Subscription`);
    
    if (!response.ok) {
        throw new Error(`Failed to fetch subscriptions: ${response.statusText}`);
    }

    return await response.json();
}

export async function getSubscriptionsByCreditCardAsync(creditCardId: string): Promise<SubscriptionView[]> {
    const response = await getAsync(`api/Subscription/credit-card/${creditCardId}`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to fetch subscriptions'));
    }

    const data = await response.json();
    return Deserializer.deserializeArray(data, SubscriptionView);
}

export async function getImmediateExpenseSubscriptionsAsync(): Promise<SubscriptionView[]> {
    const response = await getAsync(`api/Subscription/immediate-expense`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to fetch immediate expense subscriptions'));
    }

    const data = await response.json();
    return Deserializer.deserializeArray(data, SubscriptionView);
}

export async function updateSubscriptionAsync(id: string, request: SubscriptionRequest): Promise<void> {
    const response = await putAsync(`api/Subscription/${id}`, request);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to update subscription'));
    }
}

export async function deleteSubscriptionAsync(id: string): Promise<void> {
    const response = await deleteAsync(`api/Subscription/${id}`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to delete subscription'));
    }
}
