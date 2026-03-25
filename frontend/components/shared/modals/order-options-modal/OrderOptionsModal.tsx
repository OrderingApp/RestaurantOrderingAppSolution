'use client';

import { useState } from 'react';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import Image from 'next/image';
import { toast } from 'sonner';

import {
    ORDER_STATUSES,
    PAYMENT_STATUSES,
    SEARCH_PARAMS_NAMES,
} from '@/helpers/constants/constants';
import { ICONS } from '@/helpers/constants/icons/icons';
import languagePacks from '@/helpers/constants/languagePacks';
import useLanguage from '@/helpers/hooks/useLanguage';
import useQuerySingleOrder from '@/helpers/queries/orders/useQuerySingleOrder';
import useOrderMutation from '@/helpers/queries/orders/useOrdersMutation';
import { aggregateAndSortOrderItems } from '@/helpers/utils/orderTransforms';
import { formatPhoneNumber, toggleQueryParam } from '@/helpers/utils/utils';
import { formatDate } from '@/helpers/utils/dates';

import AlertDialog from '../AlertDialog';
import ActionsBar from './ActionsBar';
import InfoView from './InfoView';
import SummaryView from './SummaryView';
import { getActionButtons } from './getActionButtons';
import { VIEWS, type InformationInput, type View } from './types';

const MODAL_WIDTH = '445px';
const ORDER_TYPE = 'Takeaway';

const OrderOptionsModal = ({ onClose }: { onClose: () => void }) => {
    const { language } = useLanguage();
    const searchParms = useSearchParams();
    const router = useRouter();
    const pathname = usePathname();
    const orderId = searchParms.get(SEARCH_PARAMS_NAMES.ORDER_ID);
    const { data } = useQuerySingleOrder(orderId || '');

    const [currentView, setCurrentView] = useState<View>(VIEWS.INFO);
    const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);

    const paymentStatus = data?.paymentStatus?.toLowerCase();
    const orderStatus = data?.orderStatus?.toLowerCase();
    const isPaid = paymentStatus === PAYMENT_STATUSES.PAID.toLowerCase();
    const isClosed = orderStatus === ORDER_STATUSES.CLOSED.toLowerCase();

    const {
        ordersPage: {
            orderOptionsModal: {
                titleDelivery,
                titleTakeway,
                paymentDue,
                customerInformation: { time, phoneNumber, address, comment },
                summary: {
                    title,
                    status,
                    paid,
                    unpaid,
                    paidAmount,
                    remainingAmount,
                },
                toasts: {
                    deleteSuccess,
                    deleteError,
                    closeSuccess,
                    closeError,
                },
            },
        },
    } = languagePacks[language];

    const orderTotalAmount = data?.totalAmount ?? 0;
    const paidAmountFromOrder = data?.paidAmount;
    const remainingAmountFromOrder =
        data?.remainingAmount ?? data?.unpaidAmount;

    const hasPaidAmountFromOrder = typeof paidAmountFromOrder === 'number';
    const hasRemainingAmountFromOrder =
        typeof remainingAmountFromOrder === 'number';

    const totalPaidAmount = hasPaidAmountFromOrder
        ? Math.min(Math.max(paidAmountFromOrder, 0), orderTotalAmount)
        : isPaid
          ? orderTotalAmount
          : 0;

    const remainingAmountValue = hasRemainingAmountFromOrder
        ? Math.min(Math.max(remainingAmountFromOrder, 0), orderTotalAmount)
        : Math.max(orderTotalAmount - totalPaidAmount, 0);

    const summaryOrderItems = aggregateAndSortOrderItems(
        data?.orderItems ?? []
    );

    const informationInputs: InformationInput[] = [
        {
            label: time,
            value: formatDate(
                new Date(
                    data?.customerInformation?.expectedOrderCompletion || ''
                ),
                language
            ).time,
            icon: ICONS.TIME,
            alt: 'time',
        },
        {
            label: phoneNumber,
            value: formatPhoneNumber(
                data?.customerInformation?.phoneNumber || ''
            ),
            icon: ICONS.PHONE,
            alt: 'phone',
        },
        {
            label: address,
            value: data?.customerInformation?.address,
            icon: ICONS.MAP_MARKER,
            alt: 'address',
        },

        {
            label: comment,
            value: data?.customerInformation?.additionalInstructions,
        },
    ];

    const orderKind =
        data?.orderType === 'Delivery'
            ? 'Delivery'
            : data?.orderType === 'dinein'
              ? 'dinein'
              : 'Takeaway';

    const { mutate: deleteOrderMutation, isPending: isDeleteOrderPending } =
        useOrderMutation('delete', orderKind, {
            redirectOnSettled: false,
            onSuccess: () => {
                setIsDeleteDialogOpen(false);
                onClose();
                toast.success(deleteSuccess);
            },
            onError: () => {
                setIsDeleteDialogOpen(false);
                toast.error(deleteError);
            },
        });

    const { mutate: closeOrderMutation, isPending: isCloseOrderPending } =
        useOrderMutation('close', orderKind, {
            redirectOnSettled: false,
            onSuccess: () => {
                onClose();
                toast.success(closeSuccess);
            },
            onError: () => {
                toast.error(closeError);
            },
        });

    const handleCloseOrder = () => {
        if (!orderId) return;
        if (isClosed) return;

        if (isPaid) {
            if (isCloseOrderPending) return;

            closeOrderMutation({ id: orderId });
            return;
        }

        toggleQueryParam(
            SEARCH_PARAMS_NAMES.CLOSE_ORDER,
            'true',
            searchParms,
            router,
            pathname
        );
    };

    const handleDeleteOrder = () => {
        if (!orderId || isDeleteOrderPending) return;

        deleteOrderMutation({ id: orderId });
    };

    const handleEditOrder = () => {
        if (!orderId) return;
        if (isPaid || isClosed) return;

        toggleQueryParam(
            SEARCH_PARAMS_NAMES.MODAL,
            'true',
            searchParms,
            router,
            pathname
        );
    };

    const actionButtons = getActionButtons({
        view: currentView,
        isPaid,
        isClosed,
        onShowInfo: () => setCurrentView(VIEWS.INFO),
        onShowSummary: () => setCurrentView(VIEWS.SUMMARY),
        onCloseOrder: handleCloseOrder,
        onDeleteOrder: () => setIsDeleteDialogOpen(true),
        onEditOrder: handleEditOrder,
    });

    return (
        <div
            className="bg-order-card-gradient pt-[0.4rem] rounded-2xl relative"
            style={{ width: MODAL_WIDTH }}
        >
            <button onClick={onClose} className="absolute top-3 right-2">
                <Image className="w-6 h-6" src={ICONS.CLOSE} alt="close" />
            </button>
            <div className="bg-white  rounded-2xl h-full p-4 flex flex-col justify-center">
                <div>
                    {currentView === VIEWS.INFO && (
                        <InfoView
                            title={
                                data?.orderType === ORDER_TYPE
                                    ? titleTakeway
                                    : titleDelivery
                            }
                            paymentDueLabel={paymentDue}
                            isPaid={isPaid}
                            totalAmount={data?.totalAmount ?? 0}
                            informationInputs={informationInputs}
                        />
                    )}

                    {currentView === VIEWS.SUMMARY && (
                        <SummaryView
                            title={title}
                            statusLabel={status}
                            paidLabel={paid}
                            unpaidLabel={unpaid}
                            paidAmountLabel={paidAmount}
                            remainingAmountLabel={remainingAmount}
                            isPaid={isPaid}
                            totalAmount={data?.totalAmount ?? 0}
                            totalPaidAmount={totalPaidAmount}
                            remainingAmountValue={remainingAmountValue}
                            summaryOrderItems={summaryOrderItems}
                        />
                    )}
                </div>

                <ActionsBar view={currentView} buttons={actionButtons} />
            </div>

            <AlertDialog
                isOpen={isDeleteDialogOpen}
                onClose={() => setIsDeleteDialogOpen(false)}
                onConfirm={handleDeleteOrder}
            />
        </div>
    );
};

export default OrderOptionsModal;
