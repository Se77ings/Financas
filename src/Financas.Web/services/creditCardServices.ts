import { getAsync, postAsync, putAsync, deleteAsync, readErrorAsync } from "./baseServices";
import CreditCardView from "@/models/CreditCard/CreditCardView";
import { Deserializer } from "./Deserializer";

export interface CreditCardRequest {
    name: string;
    closingDay: number;
    dueDay: number;
    limit: number;
    isActive: boolean;
}

export async function getCreditCardsAsync(): Promise<CreditCardView[]> {
    const response = await getAsync(`api/CreditCard`);
    
    if (!response.ok) {
        throw new Error(`Failed to fetch credit cards: ${response.statusText}`);
    }

    const data = await response.json();
    return Deserializer.deserializeArray(data, CreditCardView);
}

export async function getCreditCardByIdAsync(id: string): Promise<CreditCardView> {
    const response = await getAsync(`api/CreditCard/${id}`);
    
    if (!response.ok) {
        throw new Error(`Failed to fetch credit card: ${response.statusText}`);
    }

    const data = await response.json();
    return Deserializer.deserialize(data, CreditCardView);
}

export async function addCreditCardAsync(request: CreditCardRequest): Promise<void> {
    const response = await postAsync(`api/CreditCard`, request);
    
    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to add credit card'));
    }
}

export async function updateCreditCardAsync(id: string, request: CreditCardRequest): Promise<void> {
    const response = await putAsync(`api/CreditCard/${id}`, request);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to update credit card'));
    }
}

export async function deleteCreditCardAsync(id: string): Promise<void> {
    const response = await deleteAsync(`api/CreditCard/${id}`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to delete credit card'));
    }
}
