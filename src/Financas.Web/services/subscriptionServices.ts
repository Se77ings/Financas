import { getAsync, postAsync, putAsync, deleteAsync, readErrorAsync } from "./baseServices";

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
}

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
