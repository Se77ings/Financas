import { getCategorizedDescriptionsAsync } from "./categoryServices";

/** Para cada palavra, quantas vezes ela aparece em lançamentos de cada categoria. */
type WordIndex = Map<string, Map<string, number>>;

const MIN_WORD_LENGTH = 3;

let indexPromise: Promise<WordIndex> | null = null;

// Palavras curtas e com dígitos (parcela "3/10", datas, "2x") casariam com quase tudo e gerariam sugestões erradas.
function tokenize(description: string): string[] {
    const words = description
        .normalize('NFD')
        .replace(/\p{Diacritic}/gu, '')
        .toLowerCase()
        .split(/[^a-z0-9]+/);

    return [...new Set(words.filter(word => word.length >= MIN_WORD_LENGTH && !/\d/.test(word)))];
}

function addToIndex(index: WordIndex, description: string, categoryId: string, count: number) {
    for (const word of tokenize(description)) {
        const counts = index.get(word) ?? new Map<string, number>();
        counts.set(categoryId, (counts.get(categoryId) ?? 0) + count);
        index.set(word, counts);
    }
}

function getIndex(): Promise<WordIndex> {
    if (!indexPromise) {
        indexPromise = getCategorizedDescriptionsAsync()
            .then(descriptions => {
                const index: WordIndex = new Map();
                descriptions.forEach(x => addToIndex(index, x.description, x.categoryId, x.count));
                return index;
            })
            .catch(err => {
                console.error('Erro ao carregar as descrições categorizadas:', err);
                indexPromise = null;
                return new Map();
            });
    }
    return indexPromise;
}

/**
 * Sugere a categoria pelas palavras da descrição. Quando as palavras apontam para mais de uma
 * categoria, vence a que somar mais ocorrências. Retorna nulo quando nenhuma palavra é conhecida.
 */
export async function suggestCategoryAsync(description: string): Promise<string | null> {
    const words = tokenize(description);
    if (words.length === 0) return null;

    const index = await getIndex();
    const scores = new Map<string, number>();
    for (const word of words) {
        index.get(word)?.forEach((count, categoryId) => scores.set(categoryId, (scores.get(categoryId) ?? 0) + count));
    }

    let best: string | null = null;
    let bestScore = 0;
    scores.forEach((score, categoryId) => {
        if (score > bestScore) {
            best = categoryId;
            bestScore = score;
        }
    });
    return best;
}

export function rememberCategorizedDescription(description: string, categoryId: string) {
    indexPromise?.then(index => addToIndex(index, description, categoryId, 1));
}
