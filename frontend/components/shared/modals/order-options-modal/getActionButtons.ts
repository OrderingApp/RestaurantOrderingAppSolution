import { ICONS } from '@/helpers/constants/icons/icons';

import { VIEWS, type View } from './types';
import type { ActionButton } from './ActionsBar';

export const getActionButtons = ({
    view,
    isPaid,
    isClosed,
    onShowInfo,
    onShowSummary,
    onCloseOrder,
    onDeleteOrder,
    onEditOrder,
    onSplitBill,
}: {
    view: View;
    isPaid: boolean;
    isClosed: boolean;
    onShowInfo: () => void;
    onShowSummary: () => void;
    onCloseOrder: () => void;
    onDeleteOrder: () => void;
    onEditOrder: () => void;
    onSplitBill: () => void;
}): ActionButton[] => {
    if (view === VIEWS.INFO) {
        const buttons: ActionButton[] = [
            {
                icon: ICONS.PREVIEW,
                color: '#2B5162',
                alt: 'preview',
                onClick: onShowSummary,
            },
            {
                icon: ICONS.DELETE,
                color: '#D32F2F',
                alt: 'delete',
                onClick: onDeleteOrder,
            },
        ];

        if (!isClosed && !isPaid) {
            buttons.splice(1, 0, {
                icon: ICONS.LIST_WHITE,
                color: '#2B5162',
                alt: 'split bill',
                onClick: onSplitBill,
            });
        }

        if (!isClosed) {
            buttons.splice(1, 0, {
                icon: isPaid ? ICONS.CLOSE_WHITE : ICONS.DOLLAR_WHITE,
                color: isPaid ? '#3A4A5A' : '#00A651',
                alt: isPaid ? 'close order' : 'payment',
                onClick: onCloseOrder,
            });
        }

        return buttons;
    }

    const buttons: ActionButton[] = [
        {
            icon: ICONS.USER_WHITE,
            color: '#2B5162',
            alt: 'customer data',
            onClick: onShowInfo,
        },
        {
            icon: ICONS.DELETE,
            color: '#D32F2F',
            alt: 'delete',
            onClick: onDeleteOrder,
        },
    ];

    if (!isClosed && !isPaid) {
        buttons.splice(1, 0, {
            icon: ICONS.EDIT_ORDER,
            color: '#2B5162',
            alt: 'edit',
            onClick: onEditOrder,
        });
    }

    if (!isClosed) {
        buttons.splice(2, 0, {
            icon: isPaid ? ICONS.CLOSE_WHITE : ICONS.DOLLAR_WHITE,
            color: isPaid ? '#3A4A5A' : '#00A651',
            alt: isPaid ? 'close order' : 'payment',
            onClick: onCloseOrder,
        });
    }

    return buttons;
};
