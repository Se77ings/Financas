import type { EffectiveDateSummary, EffectiveDateView } from '@/lib/api/types';

export const effectiveDateService = {
  getList: async (): Promise<EffectiveDateSummary[]> => {
    // TODO: Implement get list
    return [
      {
        id: '1',
        month: 4,
        year: 2024,
        display: '04/2024',
        balance: 1000,
        payableCount: 5,
        receivableCount: 3,
      },
    ];
  },
  getById: async (id: string): Promise<EffectiveDateView> => {
    // TODO: Implement get by id
    return { receivable: [], payable: [], cardExpense: [] };
  },
  delete: async (id: string) => {
    // TODO: Implement delete logic
    console.log('Deleting effective date:', id);
  },
};