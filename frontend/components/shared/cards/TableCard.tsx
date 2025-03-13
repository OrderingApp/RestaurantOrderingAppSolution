'use client';

import { useLanguage } from '@/providers/LanguageProvider';

import ItemCard, { ItemCardProps } from './ItemCard';

import { variantTableMap } from '@/lib/styles/itemCard';
import languagePacks from '@/helpers/constants/languagePacks';

interface TableCardProps
    extends Omit<ItemCardProps, 'variant' | 'children' | 'title'> {
    status: keyof typeof variantTableMap;
    balance?: string;
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
            title={status === 'payment' ? payment : normal}
            variant={variantTableMap[status]}
            onClick={onClick}
        >
            <div>
                <p className="text-left font-bold text-[11px]">
                    {balanceName}: {balance}
                </p>
            </div>
        </ItemCard>
    );
};

export default TableCard;
