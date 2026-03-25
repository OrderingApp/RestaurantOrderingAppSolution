'use client';

import DetailsAside from '@/components/shared/asides/Details';
import Menu from '../../pages/menu/Menu';
import {
    COMPANYS_CURRENCY,
    ORDER_TYPES,
    ordersTypes,
    SEARCH_PARAMS_NAMES,
} from '@/helpers/constants/constants';
import { useOrdersContext } from '@/providers/OrdersContext';
import { OrdersItems } from '@/helpers/utils/queryKeys';
import { useState, useMemo } from 'react';
import { usePathname, useRouter, useSearchParams } from 'next/navigation';
import { toggleQueryParam } from '@/helpers/utils/utils';
import CustomerInformationForm from '../../pages/orders/CustomerInformationForm';
import { ItemProps } from '@/components/shared/lists/Items/Item';
import { ButtonProps } from '@/components/shared/button/Button';
import { Currency } from '@/helpers/type/types';
import { useLanguage } from '@/providers/LanguageProvider';
import languagePacks from '@/helpers/constants/languagePacks';
import useQueryOrders from '@/helpers/queries/orders/useQueryOrders';
import useQueryTables from '@/helpers/queries/tables/useQueryTables';
import { getAggregatedDineInOrdersForTable } from '@/helpers/utils/orderTransforms';
import useConfirmDineinBillsMutation from '@/helpers/queries/orders/useConfirmDineinBillsMutation';
import useQuerySingleOrder from '@/helpers/queries/orders/useQuerySingleOrder';
import {
    useQueryMenuIngredients,
    useQueryMenuItem,
} from '@/helpers/queries/menu-items/useQueryMenuItems';
import type { AddItemHandler } from '@/components/shared/cards/MenuItem';
import { formatPriceStr } from '@/helpers/utils/prices';
import { useCreateOrderLocalBills } from '../../../helpers/hooks/useCreateOrderLocalBills';
import SwipeableIngredientRow from '../animations/SwipeableIngredientRow';

export interface BillProps {
    id: string;
    name: string;
    price: number;
    currency: Currency;
    nestedItems: ItemProps[];
    className?: string;
}

const CreateOrder = ({
    toggleModal: _toggleModal,
    skipCustomerForm = false,
    tableId,
    orderType,
    allowMultipleBills,
}: {
    toggleModal: () => void;
    skipCustomerForm?: boolean;
    tableId?: string | undefined;
    orderType?: ORDER_TYPES;
    allowMultipleBills?: boolean;
}) => {
    const {
        clearOrders,
        clearExtraIngredients,
        clearRemovedIngredients,
        deliveryPrice,
        addExtraIngredient,
        decrementExtraIngredient,
        removeIngredientFromOrderItem,
        restoreIngredientForOrderItem,
        getExtraIngredients,
        getExtraIngredientsTotalPrice,
        getExtraIngredientsPayloadByOrderItemId,
        getRemovedIngredientIds,
        getRemovedIngredientIdsPayloadByOrderItemId,
    } = useOrdersContext();

    const router = useRouter();
    const pathname = usePathname();
    const searchParams = useSearchParams();
    const selectedOrderItemId = searchParams.get(
        SEARCH_PARAMS_NAMES.MENU_ITEM_ID
    );
    const editedOrderId = searchParams.get(SEARCH_PARAMS_NAMES.ORDER_ID);
    const userData = searchParams.get(SEARCH_PARAMS_NAMES.USER_DATA);
    const isEditMode = skipCustomerForm && !!editedOrderId;

    const [selectedId, setSelectedId] = useState('');

    const { language } = useLanguage();

    const { data: allOrders, isLoading: isOrdersLoading } = useQueryOrders({
        queryKeys: [OrdersItems.BY_TYPE, ORDER_TYPES.DINEIN],
    });
    const { data: editedOrder, isLoading: isEditedOrderLoading } =
        useQuerySingleOrder(editedOrderId || '');
    const { data: tables } = useQueryTables();

    const tableName =
        tableId && tables
            ? tables.find((t) => t.id === tableId)?.name || ''
            : '';

    const existingOrdersForTable = useMemo(() => {
        if (isEditMode) {
            if (!editedOrder) return [];

            return [
                {
                    id: editedOrder.id,
                    totalAmount: editedOrder.totalAmount,
                    orderItems: editedOrder.orderItems.map((item) => ({
                        id: item.id,
                        price: item.price,
                        quantity: 1,
                        discount: item.discount,
                        annotation: [
                            ...(item.extraIngredients?.map(
                                (extra) =>
                                    `+ ${extra.ingredientName}${extra.quantity > 1 ? ` x${extra.quantity}` : ''}`
                            ) ?? []),
                            ...(item.removedIngredients?.map(
                                (removed) => `- ${removed.name}`
                            ) ?? []),
                        ],
                        annotationClassName: 'text-dark-gray font-normal',
                        menuItem: {
                            id: item.menuItem.id,
                            name: item.menuItem.name,
                        },
                    })),
                },
            ];
        }

        return getAggregatedDineInOrdersForTable(allOrders || [], tableId);
    }, [allOrders, tableId, isEditMode, editedOrder]);

    const canAddMultipleBills = isEditMode
        ? false
        : (allowMultipleBills ?? !!tableId);
    const isAsideOrdersLoadingForInit = isEditMode
        ? isEditedOrderLoading
        : !!tableId && isOrdersLoading;

    const {
        localBills,
        selectedBill,
        setSelectedBill,
        selectedBillData,
        addItemToSelectedBill,
        handleAddNewBill,
        resetBillsState,
        mergeDuplicatePendingItemsForSelectedBill,
    } = useCreateOrderLocalBills({
        existingOrdersForTable,
        isAsideOrdersLoading: isAsideOrdersLoadingForInit,
        canAddMultipleBills,
        getHasExtras: (orderItemId) =>
            getExtraIngredients(orderItemId).length > 0 ||
            getRemovedIngredientIds(orderItemId).length > 0,
        getExtrasSignature: (orderItemId) => {
            const extras = getExtraIngredients(orderItemId);
            const removed = getRemovedIngredientIds(orderItemId);

            const extrasSig = extras.length
                ? [...extras]
                      .sort((a, b) =>
                          a.ingredientId.localeCompare(b.ingredientId)
                      )
                      .map((x) => `${x.ingredientId}:${x.quantity}`)
                      .join('|')
                : '';

            const removedSig = removed.length
                ? [...removed].sort((a, b) => a.localeCompare(b)).join('|')
                : '';

            return `${extrasSig}||removed:${removedSig}`;
        },
    });

    const isAsideOrdersLoading =
        (isEditMode ? isEditedOrderLoading : !!tableId && isOrdersLoading) &&
        localBills.length === 0;

    const selectedPendingItem = useMemo(() => {
        if (!selectedOrderItemId) return undefined;

        for (const bill of localBills) {
            const found = bill.pendingItems.find(
                (x) => x.id === selectedOrderItemId
            );
            if (found) return found;
        }

        return undefined;
    }, [localBills, selectedOrderItemId]);

    const selectedMenuItemId = selectedPendingItem?.menuItemId ?? '';

    const { data: selectedMenuItem, isLoading: isSelectedMenuItemLoading } =
        useQueryMenuItem(selectedMenuItemId);

    const { data: allIngredients = [] } = useQueryMenuIngredients();

    const ingredientPriceById = useMemo(() => {
        const map = new Map<string, number>();
        for (const ing of allIngredients) {
            map.set(ing.id, ing.price);
        }
        return map;
    }, [allIngredients]);

    const { detailsAside } = languagePacks[language];
    const {
        createOrderPage: {
            asideButtons: { accept, close, discount },
            extras: { title: extrasTitle, emptyFallback: noExtrasFallback },
        },
    } = languagePacks[language];

    const effectiveOrderType =
        orderType ??
        ((isEditMode
            ? editedOrder?.orderType
            : tableId
              ? ORDER_TYPES.DINEIN
              : ORDER_TYPES.TAKEAWAY) as ORDER_TYPES | undefined) ??
        ORDER_TYPES.TAKEAWAY;

    const selectedOrderTypeLabel =
        ordersTypes[language].find((type) => type.id === effectiveOrderType)
            ?.name ??
        ordersTypes[language][0]?.name ??
        detailsAside.receipt;

    const handleClose = () => {
        clearOrders();
        resetBillsState();

        // This modal is used in two contexts:
        // - Orders page: open/close is controlled by the `modal=true` URL param.
        // - Tables page: open/close is controlled by local component state.
        // In the latter case we must call the passed toggle handler, otherwise the modal stays open.
        const isUrlControlledModal =
            searchParams.get(SEARCH_PARAMS_NAMES.MODAL) === 'true';
        if (!isUrlControlledModal) {
            _toggleModal();
        }

        const newParams = new URLSearchParams(searchParams.toString());
        newParams.delete(SEARCH_PARAMS_NAMES.MODAL);
        newParams.delete(SEARCH_PARAMS_NAMES.MENU_ITEM_ID);
        newParams.delete(SEARCH_PARAMS_NAMES.USER_DATA);
        newParams.delete(SEARCH_PARAMS_NAMES.CATEGORY);
        newParams.delete(SEARCH_PARAMS_NAMES.SUBCATEGORY);
        newParams.delete(SEARCH_PARAMS_NAMES.TAG);
        newParams.delete(SEARCH_PARAMS_NAMES.ORDER_MENU_PAGE);
        newParams.delete(SEARCH_PARAMS_NAMES.PAGE);

        const queryString = newParams.toString();
        router.push(queryString ? `${pathname}?${queryString}` : pathname);
    };

    const clearSelectedMenuItem = () => {
        toggleQueryParam(
            SEARCH_PARAMS_NAMES.MENU_ITEM_ID,
            undefined,
            searchParams,
            router,
            pathname
        );
    };

    const handleAddExtraIngredient: AddItemHandler = (ingredient) => {
        if (!selectedOrderItemId) return;

        addExtraIngredient({
            orderItemId: selectedOrderItemId,
            ingredientId: ingredient.id,
            ingredientName: ingredient.name,
            price: ingredient.price,
            quantity: ingredient.quantity,
        });
    };

    const confirmDineinBillsMutation = useConfirmDineinBillsMutation({
        onSuccess: handleClose,
    });

    // Compute if accept button should be enabled:
    // only enable when there are unsynced changes (pending items) in any bill.
    // This keeps it disabled when nothing changed, or when a new bill is empty.
    const hasPendingChanges = localBills.some(
        (bill) => bill.pendingItems.length > 0
    );
    const canAcceptBill = isEditMode
        ? true
        : selectedOrderItemId
          ? true
          : !!selectedBill && hasPendingChanges;

    const buttons: ButtonProps[] = [
        {
            children: discount,
            variant: 'primary',
        },
        {
            children: accept,
            variant: 'primary',
            disabled: !canAcceptBill || confirmDineinBillsMutation.isPending,
            onClick: async () => {
                // If we're editing extras for a selected menu item, just confirm UI changes
                // and return back to the receipt view.
                if (selectedOrderItemId) {
                    const removedOrderItemIds =
                        mergeDuplicatePendingItemsForSelectedBill();
                    removedOrderItemIds.forEach((id) =>
                        clearExtraIngredients(id)
                    );
                    removedOrderItemIds.forEach((id) =>
                        clearRemovedIngredients(id)
                    );
                    clearSelectedMenuItem();
                    return;
                }

                // Guard: must have a selected bill
                if (!selectedBill) return;

                if (tableId) {
                    // For table context, submit directly to backend.
                    return await confirmDineinBillsMutation.mutateAsync({
                        bills: localBills,
                        tableId,
                        deliveryPrice: deliveryPrice || 0,
                        extraIngredientsByOrderItemId:
                            getExtraIngredientsPayloadByOrderItemId(),
                        removedIngredientIdsByOrderItemId:
                            getRemovedIngredientIdsPayloadByOrderItemId(),
                    });
                }

                if (skipCustomerForm) {
                    // For edit flow from Orders page, allow editing customer data before final save.
                    toggleUserDataModal();
                    return;
                }

                // For non-dine-in or when skipCustomerForm is false, show customer form
                toggleUserDataModal();
            },
        },
        {
            children: close,
            onClick: handleClose,
            variant: 'tertiary',
        },
    ];

    const getBillStyles = (isSelected: boolean) => ({
        className: isSelected ? 'bg-primary text-white' : '',
        priceStrClassName: isSelected ? 'text-white' : '',
    });

    const bill: BillProps[] = localBills.map((localBill, index) => {
        const isSelected = selectedBill === localBill.id;
        const pendingPrice = localBill.pendingItems.reduce((acc, item) => {
            const extrasTotalPrice = getExtraIngredientsTotalPrice(item.id);
            const safeExtrasTotalPrice = Number.isFinite(extrasTotalPrice)
                ? extrasTotalPrice
                : 0;
            return acc + (item.price + safeExtrasTotalPrice) * item.quantity;
        }, 0);
        return {
            id: localBill.id,
            name: localBill.isNew
                ? `${detailsAside.receipt} ${localBills.filter((b) => b.isNew).indexOf(localBill) + 1 + existingOrdersForTable.length} (nowy)`
                : `${detailsAside.receipt} ${index + 1}`,
            price: localBill.totalAmount + pendingPrice,
            currency: COMPANYS_CURRENCY,
            nestedItems: [
                ...localBill.orderItems.map((it) => ({
                    name: it.name,
                    price: it.price,
                    currency: COMPANYS_CURRENCY,
                    quantity: it.quantity || 1,
                    annotation: it.annotation,
                    annotationClassName: it.annotationClassName,
                    onClick: () => {},
                })),
                ...localBill.pendingItems.map((item) => {
                    const extras = getExtraIngredients(item.id);
                    const extrasTotalPrice = getExtraIngredientsTotalPrice(
                        item.id
                    );

                    return {
                        name: item.name,
                        price: item.price + extrasTotalPrice,
                        currency: COMPANYS_CURRENCY,
                        quantity: item.quantity,
                        annotation: extras.length
                            ? extras.map(
                                  (x) =>
                                      `+ ${x.ingredientName}${x.quantity > 1 ? ` x${x.quantity}` : ''}`
                              )
                            : undefined,
                        annotationClassName: extras.length
                            ? 'text-dark-gray font-normal'
                            : undefined,
                        onClick: () => toggleSelect(item.id),
                        className:
                            selectedOrderItemId === item.id ? 'bg-red-200' : '',
                    };
                }),
            ],
            ...getBillStyles(isSelected),
        };
    });

    const toggleSelect = (id: string) => {
        toggleQueryParam(
            SEARCH_PARAMS_NAMES.MENU_ITEM_ID,
            id,
            searchParams,
            router,
            pathname
        );

        if (selectedId === id) {
            setSelectedId('');
            return;
        }
        setSelectedId(id);
    };

    const toggleUserDataModal = () => {
        toggleQueryParam(
            SEARCH_PARAMS_NAMES.USER_DATA,
            'true',
            searchParams,
            router,
            pathname
        );
    };

    // Build orderItems for CustomerInformationForm in API payload shape.
    const orderItems = selectedBillData
        ? selectedBillData.pendingItems.flatMap((item) =>
              Array.from({ length: item.quantity }, () => ({
                  specialInstructions: '',
                  discount: item.discount ?? 0,
                  menuItemId: item.menuItemId,
                  extraIngredients: getExtraIngredients(item.id).map(
                      (extraIngredient) => ({
                          ingredientId: extraIngredient.ingredientId,
                          quantity: extraIngredient.quantity,
                      })
                  ),
                  removedIngredientIds: getRemovedIngredientIds(item.id),
              }))
          )
        : [];

    const selectedExtras = selectedOrderItemId
        ? getExtraIngredients(selectedOrderItemId)
        : [];
    const selectedExtrasTotalPrice = selectedOrderItemId
        ? getExtraIngredientsTotalPrice(selectedOrderItemId)
        : 0;
    const safeSelectedExtrasTotalPrice = Number.isFinite(
        selectedExtrasTotalPrice
    )
        ? selectedExtrasTotalPrice
        : 0;
    const selectedMenuItemDisplayPrice =
        (selectedMenuItem?.price ?? 0) + safeSelectedExtrasTotalPrice;

    return !userData ? (
        <Menu
            variant="order"
            onAddItem={addItemToSelectedBill}
            onAddIngredient={handleAddExtraIngredient}
        >
            {selectedOrderItemId ? (
                <DetailsAside
                    title={selectedMenuItem?.name || ''}
                    price={selectedMenuItemDisplayPrice}
                    currency={COMPANYS_CURRENCY}
                    button={{
                        onClick: clearSelectedMenuItem,
                        children: languagePacks[language].generic.close,
                    }}
                    buttons={buttons}
                    isItemsLoading={isSelectedMenuItemLoading}
                >
                    <div className="flex-1 pb-2 overflow-y-auto">
                        <section className="mb-4">
                            {selectedMenuItem?.ingredients?.length ? (
                                <ul className="">
                                    {selectedMenuItem.ingredients.map((ing) => (
                                        <SwipeableIngredientRow
                                            key={ing.id}
                                            className="flex items-center justify-center w-full p-2 odd:bg-primary even:bg-primary-light"
                                            onSwipeLeft={() => {
                                                if (!selectedOrderItemId)
                                                    return;
                                                removeIngredientFromOrderItem({
                                                    orderItemId:
                                                        selectedOrderItemId,
                                                    ingredientId: ing.id,
                                                });
                                            }}
                                            onSwipeRight={() => {
                                                if (!selectedOrderItemId)
                                                    return;

                                                const isRemoved =
                                                    getRemovedIngredientIds(
                                                        selectedOrderItemId
                                                    ).includes(ing.id);

                                                if (isRemoved) {
                                                    restoreIngredientForOrderItem(
                                                        {
                                                            orderItemId:
                                                                selectedOrderItemId,
                                                            ingredientId:
                                                                ing.id,
                                                        }
                                                    );
                                                    return;
                                                }

                                                // Swipe right on a base ingredient adds an extra of the same ingredient (e.g. double cheese)
                                                addExtraIngredient({
                                                    orderItemId:
                                                        selectedOrderItemId,
                                                    ingredientId: ing.id,
                                                    ingredientName: ing.name,
                                                    price:
                                                        ingredientPriceById.get(
                                                            ing.id
                                                        ) ?? 0,
                                                    quantity: 1,
                                                });
                                            }}
                                        >
                                            <span
                                                className={
                                                    getRemovedIngredientIds(
                                                        selectedOrderItemId
                                                    ).includes(ing.id)
                                                        ? 'text-md text-white line-through opacity-60'
                                                        : 'text-md text-white'
                                                }
                                            >
                                                {ing.name}
                                            </span>
                                        </SwipeableIngredientRow>
                                    ))}
                                </ul>
                            ) : (
                                <p className="text-sm text-center text-black/70">
                                    {languagePacks[language].generic.noResults}
                                </p>
                            )}
                        </section>

                        <section className="mt-6">
                            <h3 className="text-md py-2 text-center border-gray border-t-2 border-b-2 font-semibold ">
                                {extrasTitle}
                            </h3>

                            {selectedExtras.length ? (
                                <ul>
                                    {selectedExtras.map((extra) => (
                                        <SwipeableIngredientRow
                                            key={extra.ingredientId}
                                            className="flex items-center justify-center gap-4 w-full p-2 odd:bg-primary even:bg-primary-light"
                                            onSwipeLeft={() => {
                                                if (!selectedOrderItemId)
                                                    return;
                                                decrementExtraIngredient({
                                                    orderItemId:
                                                        selectedOrderItemId,
                                                    ingredientId:
                                                        extra.ingredientId,
                                                    quantity: 1,
                                                });
                                            }}
                                            onSwipeRight={() => {
                                                if (!selectedOrderItemId)
                                                    return;
                                                addExtraIngredient({
                                                    orderItemId:
                                                        selectedOrderItemId,
                                                    ingredientId:
                                                        extra.ingredientId,
                                                    ingredientName:
                                                        extra.ingredientName,
                                                    price: extra.price,
                                                    quantity: 1,
                                                });
                                            }}
                                        >
                                            <span className="text-md text-white">
                                                {extra.ingredientName}
                                                {extra.quantity > 1
                                                    ? ` x${extra.quantity}`
                                                    : ''}
                                            </span>
                                            <span className="font-semibold text-slate-300">
                                                {formatPriceStr({
                                                    currency: COMPANYS_CURRENCY,
                                                    price: extra.price,
                                                    quantity: extra.quantity,
                                                })}
                                            </span>
                                        </SwipeableIngredientRow>
                                    ))}
                                </ul>
                            ) : (
                                <p className="text-sm text-center text-black/70">
                                    {noExtrasFallback}
                                </p>
                            )}
                        </section>
                    </div>
                </DetailsAside>
            ) : (
                <DetailsAside
                    title={
                        tableName
                            ? `${detailsAside.table} ${tableName}`
                            : selectedOrderTypeLabel
                    }
                    items={isAsideOrdersLoading ? [] : bill}
                    buttons={buttons}
                    onSelectItem={setSelectedBill}
                    selectedItemId={selectedBill}
                    onAddNewOrder={
                        canAddMultipleBills ? handleAddNewBill : undefined
                    }
                    isItemsLoading={isAsideOrdersLoading}
                />
            )}
        </Menu>
    ) : (
        <CustomerInformationForm
            bill={bill}
            orderItems={orderItems}
            editedOrderId={isEditMode ? editedOrderId || undefined : undefined}
            editedCustomerInformation={
                isEditMode ? editedOrder?.customerInformation : undefined
            }
            editedOrderType={isEditMode ? editedOrder?.orderType : undefined}
        />
    );
};

export default CreateOrder;
