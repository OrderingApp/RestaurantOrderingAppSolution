'use client';

import { useState } from 'react';
import { usePathname, useSearchParams } from 'next/navigation';
import { useRouter } from 'next/navigation';
import Image from 'next/image';

import { SEARCH_PARAMS_NAMES, CURRENCIES } from '@/helpers/constants/constants';
import { ICONS } from '@/helpers/constants/icons/icons';
import languagePacks from '@/helpers/constants/languagePacks';
import useLanguage from '@/helpers/hooks/useLanguage';
import useQuerySingleOrder from '@/helpers/queries/orders/useQuerySingleOrder';
import { aggregateAndSortOrderItems } from '@/helpers/utils/orderTransforms';
import { formatPhoneNumber, toggleQueryParam } from '@/helpers/utils/utils';
import clsx from 'clsx';
import { inputStyles } from '@/lib/styles/input';
import { formatDate } from '@/helpers/utils/dates';

const VIEWS = {
    INFO: 'info',
    SUMMARY: 'summary',
};

const MODAL_WIDTH = '445px';
const ORDER_TYPE = 'Takeaway';

const OrderOptionsModal = ({ onClose }: { onClose: () => void }) => {
    const { language } = useLanguage();
    const searchParms = useSearchParams();
    const router = useRouter();
    const pathname = usePathname();
    const orderId = searchParms.get(SEARCH_PARAMS_NAMES.ORDER_ID);
    const { data } = useQuerySingleOrder(orderId || '');

    const [currentView, setCurrentView] = useState(VIEWS.INFO);

    const isPaid = data?.paymentStatus === 'Paid';

    const {
        ordersPage: {
            orderOptionsModal: {
                titleDelivery,
                titleTakeway,
                paymentDue,
                customerInformation: { time, phoneNumber, address, comment },
                summary: { title, status, paid, unpaid },
            },
        },
    } = languagePacks[language];

    const summaryOrderItems = aggregateAndSortOrderItems(
        data?.orderItems ?? []
    );

    const informationInputs = [
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

    const closeOrder = () => {
        toggleQueryParam(
            SEARCH_PARAMS_NAMES.CLOSE_ORDER,
            'true',
            searchParms,
            router,
            pathname
        );
    };

    const getActionsButtons = () => {
        if (currentView === VIEWS.INFO) {
            return isPaid
                ? [
                      {
                          icon: ICONS.PREVIEW,
                          color: '#2B5162',
                          alt: 'preview',
                          onClick: () => setCurrentView(VIEWS.SUMMARY),
                      },
                      {
                          icon: ICONS.CLOSE,
                          color: '#3A4A5A',
                          alt: 'close order',
                          onClick: closeOrder,
                      },
                      {
                          icon: ICONS.DELETE,
                          color: '#D32F2F',
                          alt: 'delete',
                          onClick: () => console.log('Delete'),
                      },
                  ]
                : [
                      {
                          icon: ICONS.PREVIEW,
                          color: '#2B5162',
                          alt: 'preview',
                          onClick: () => setCurrentView(VIEWS.SUMMARY),
                      },
                      {
                          icon: ICONS.DOLLAR_WHITE,
                          color: '#00A651',
                          alt: 'payment',
                          onClick: closeOrder,
                      },
                      {
                          icon: ICONS.DELETE,
                          color: '#D32F2F',
                          alt: 'delete',
                          onClick: () => console.log('Delete'),
                      },
                  ];
        } else {
            return isPaid
                ? [
                      {
                          icon: ICONS.USER,
                          color: '#2B5162',
                          alt: 'customer data',
                          onClick: () => setCurrentView(VIEWS.INFO),
                      },
                      {
                          icon: ICONS.USER,
                          color: '#2B5162',
                          alt: 'preview no edit',
                          onClick: () => console.log('Preview no edit'),
                      },
                      {
                          icon: ICONS.CLOSE,
                          color: '#3A4A5A',
                          alt: 'close order',
                          onClick: closeOrder,
                      },
                      {
                          icon: ICONS.DELETE,
                          color: '#D32F2F',
                          alt: 'delete',
                          onClick: () => console.log('Delete'),
                      },
                  ]
                : [
                      {
                          icon: ICONS.USER_WHITE,
                          color: '#2B5162',
                          alt: 'customer data',
                          onClick: () => setCurrentView(VIEWS.INFO),
                      },
                      {
                          icon: ICONS.EDIT_ORDER,
                          color: '#2B5162',
                          alt: 'edit',
                          onClick: () => console.log('Edit'),
                      },
                      {
                          icon: ICONS.DOLLAR_WHITE,
                          color: '#00A651',
                          alt: 'payment',
                          onClick: closeOrder,
                      },
                      {
                          icon: ICONS.DELETE,
                          color: '#D32F2F',
                          alt: 'delete',
                          onClick: () => console.log('Delete'),
                      },
                  ];
        }
    };

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
                        <>
                            <div className="flex justify-between items-start">
                                <h2 className="text-xl font-bold">
                                    {data?.orderType === ORDER_TYPE
                                        ? titleTakeway
                                        : titleDelivery}
                                </h2>
                                <div className="bg-[#F6F6F6] flex justify-center items-center p-2 px-3 gap-3 mr-8 rounded-lg shadow-sm border border-gray-100">
                                    <span className="font-bold text-sm">
                                        {paymentDue}
                                    </span>
                                    <span
                                        className={clsx('font-bold', {
                                            'text-paid': isPaid,
                                            'text-ongoing': !isPaid,
                                        })}
                                    >
                                        {data?.totalAmount} {CURRENCIES.pln}
                                    </span>
                                </div>
                            </div>
                            <ul className="flex flex-col gap-2 mt-2">
                                {informationInputs.map((input) => (
                                    <li key={input.label}>
                                        <h4 className="font-bold ml-2 text-sm">
                                            {input.label}
                                        </h4>
                                        <div
                                            className={clsx(
                                                inputStyles.variants.primary,
                                                'flex h-11 rounded-2xl p-3 justify-between items-center'
                                            )}
                                        >
                                            <span className="text-sm">
                                                {input.value}
                                            </span>
                                            {input.icon && (
                                                <Image
                                                    className="aspect-square"
                                                    src={input.icon}
                                                    alt={input.alt ?? ''}
                                                    width={22}
                                                    height={22}
                                                />
                                            )}
                                        </div>
                                    </li>
                                ))}
                            </ul>
                        </>
                    )}

                    {currentView === VIEWS.SUMMARY && (
                        <>
                            <h2 className="text-xl font-bold mb-8">{title}</h2>

                            <div className="border border-gray-300 rounded-2xl p-2 py-4 shadow-xl">
                                <ul className="flex flex-col px-2">
                                    {summaryOrderItems.map((item, index) => (
                                        <li
                                            key={`${item.id}-${index}`}
                                            className="flex justify-between items-center py-2 pb-1 border-b border-gray-300 text-md font-bold text-black"
                                        >
                                            <span className="font-bold">
                                                {item.menuItem.name}{' '}
                                                <span className="text-dark-gray font-normal text-sm ml-1">
                                                    x{item.quantity}
                                                </span>
                                            </span>
                                            <span>
                                                {item.menuItem.price} zł
                                            </span>
                                        </li>
                                    ))}
                                </ul>

                                <div className="mt-5 bg-gray font-semibold text-center py-3 rounded-xl text-xl text-black">
                                    <span className=""> {status}: </span>

                                    <span
                                        className={
                                            isPaid
                                                ? 'text-paid'
                                                : 'text-ongoing'
                                        }
                                    >
                                        {isPaid ? paid : unpaid}
                                    </span>
                                </div>
                            </div>
                        </>
                    )}
                </div>

                <ul
                    className={`flex gap-4 py-3 justify-between mt-6 ${currentView == VIEWS.INFO ? 'px-12' : 'px-6'}`}
                >
                    {getActionsButtons().map((btn) => (
                        <li key={btn.alt}>
                            <button
                                onClick={btn.onClick}
                                style={{ backgroundColor: btn.color }}
                                className="w-16 h-16 rounded-lg shadow-md flex justify-center items-center hover:opacity-90 transition-opacity"
                            >
                                <Image
                                    className="w-8 h-8"
                                    src={btn.icon}
                                    alt={btn.alt}
                                />
                            </button>
                        </li>
                    ))}
                </ul>
            </div>
        </div>
    );
};

export default OrderOptionsModal;
