'use client';

import { useLanguage } from '@/providers/LanguageProvider';

import ItemCard, { ItemCardProps } from './ItemCard';

import { variantTableMap } from '@/lib/styles/itemCard';
import languagePacks from '@/helpers/constants/languagePacks';
import { CURRENCIES } from '@/helpers/constants/constants';

interface TableCardProps
    extends Omit<ItemCardProps, 'variant' | 'children' | 'title'> {
    status: keyof typeof variantTableMap;
    balance: number;
}

const TableCard = ({ status, balance, onClick }: TableCardProps) => {
    const { language } = useLanguage();
    const {
        tablePage: {
            tableCard: {
                title: { payment, normal },
                balanceName,
            },
        },
    } = languagePacks[language];

    return (
        <ItemCard
            title={status === 'PAYMENT' ? payment : normal}
            variant={variantTableMap[status]}
            onClick={onClick}
        >
            <p className="text-left font-bold text-[11px]">
                {balanceName}:{balance}
                {CURRENCIES.pln}
            </p>
        </ItemCard>
    );
};

export default TableCard;

//TODO CHANGE CURRENCIES
