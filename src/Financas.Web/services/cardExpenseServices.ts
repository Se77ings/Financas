import { getAsync, postAsync, putAsync, deleteAsync, readErrorAsync } from "./baseServices";
import CardExpenseSummary from "@/models/CardExpense/CardExpenseSummary";
import { Deserializer } from "./Deserializer";

export interface CardExpenseRequest {
    description: string;
    dueDate: string;
    subTotal: number;
    discounts: number;
    observation: string;
    effectiveDateId: string;
}

export interface CardExpenseItemRequest {
    description: string;
    dueDate: string;
    subTotal: number;
    discounts: number;
    observation: string;
    currentInstallment: number;
    quantityInstallments: number;
    categoryId: string | null;
    subscriptionFrequencyType?: number;
    subscriptionStartDate?: string;
    subscriptionEndDate?: string | null;
    isSubscription?: boolean;
}

export async function getCardExpenseAsync(cardExpenseId: string): Promise<CardExpenseSummary> {
    const response = await getAsync(`api/CardExpense/${cardExpenseId}`);
    
    if (!response.ok) {
        throw new Error(`Failed to fetch card expense: ${response.statusText}`);
    }

    const data = await response.json();
    
    return Deserializer.deserialize(data, CardExpenseSummary);
}

export async function getCardExpensesAsync(): Promise<CardExpenseSummary[]> {
    const response = await getAsync(`api/CardExpense`);
    
    if (!response.ok) {
        throw new Error(`Failed to fetch card expenses: ${response.statusText}`);
    }

    const data = await response.json();
    
    return Deserializer.deserializeArray(data, CardExpenseSummary);
}

export async function addCardExpenseAsync(request: CardExpenseRequest): Promise<void> {
    const response = await postAsync(`api/CardExpense`, request);
    
    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to add card expense'));
    }
}

export async function updateCardExpenseAsync(id: string, request: CardExpenseRequest): Promise<void> {
    const response = await putAsync(`api/CardExpense/${id}`, request);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to update card expense'));
    }
}

export async function deleteCardExpenseAsync(id: string): Promise<void> {
    const response = await deleteAsync(`api/CardExpense/${id}`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to delete card expense'));
    }
}

export async function addCardExpenseItemAsync(cardExpenseId: string, request: CardExpenseItemRequest): Promise<void> {
    const response = await postAsync(`api/CardExpense/${cardExpenseId}/items`, request);
    
    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to add card expense item'));
    }
}

export async function updateCardExpenseItemAsync(cardExpenseId: string, itemId: string, request: CardExpenseItemRequest): Promise<void> {
    const response = await putAsync(`api/CardExpense/${cardExpenseId}/items/${itemId}`, request);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to update card expense item'));
    }
}

export async function deleteCardExpenseItemAsync(cardExpenseId: string, itemId: string): Promise<void> {
    const response = await deleteAsync(`api/CardExpense/${cardExpenseId}/items/${itemId}`);

    if (!response.ok) {
        throw new Error(await readErrorAsync(response, 'Failed to delete card expense item'));
    }
}

export async function transferCardExpenseItemsAsync(itemIds: string[], targetEffectiveDateId: string): Promise<void> {
    const response = await postAsync(`api/CardExpense/transfer-items`, {
        cardExpenseItemIds: itemIds,
        targetEffectiveDateId
    });
    
    if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || 'Failed to transfer card items');
    }
}
