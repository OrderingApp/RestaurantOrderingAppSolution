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
import { toggleQueryParam } from '@/helpers/utils/utils';

const VIEWS = {
    INFO: 'info',
    SUMMARY: 'summary',
};

const OrderOptionsModal = ({ onClose }: { onClose: () => void }) => {
    const { language } = useLanguage();
    const searchParms = useSearchParams();
    const router = useRouter();
    const pathname = usePathname();
    const orderId = searchParms.get(SEARCH_PARAMS_NAMES.ORDER_ID);
    const { data } = useQuerySingleOrder(orderId || '');

    // --- NOWE STANY I ZMIENNE ---
    const [currentView, setCurrentView] = useState(VIEWS.INFO);

    // Zastąp to właściwym polem z Twojego API sprawdzającym czy opłacone
    const isPaid = data?.isPaid || false;

    const {
        ordersPage: {
            orderOptionsModal: {
                titleDelivery,
                titleTakeway,
                paymentDue,
                customerInformation: { time, phoneNumber, address },
            },
        },
    } = languagePacks[language];

    // Informacje o kliencie (Widok Info)
    const informationInputs = [
        {
            label: 'Dane', // Warto dodać do paczki językowej
            value: data?.customerInformation?.name || 'Jan Kowalski',
            icon: ICONS.USER, // Upewnij się, że masz taką ikonę
            alt: 'user',
        },
        {
            label: time,
            value: data?.customerInformation?.expectedOrderCompletion
                ?.split('T')[1]
                ?.slice(0, 5),
            icon: ICONS.TIME,
            alt: 'time',
        },
        {
            label: phoneNumber,
            value: data?.customerInformation?.phoneNumber,
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
            label: 'Komentarz',
            value: data?.customerInformation?.comment || 'Brak',
            icon: ICONS.COMMENT,
            alt: 'comment',
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

    // --- LOGIKA PRZYCISKÓW ---
    // Przyciski zmieniają się w zależności od widoku i statusu opłacenia
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
            // WIDOK PODSUMOWANIA
            return isPaid
                ? [
                      {
                          icon: ICONS.USER,
                          color: '#2B5162',
                          alt: 'customer data',
                          onClick: () => setCurrentView(VIEWS.INFO),
                      },
                      {
                          icon: ICONS.PREVIEW_NO_EDIT,
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
                          icon: ICONS.USER,
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

    const MODAL_WIDTH = '445px';
    const ORDER_TYPE = 'Takeaway';

    return (
        <div
            className="bg-order-card-gradient pt-[0.4rem] rounded-2xl relative"
            style={{ width: MODAL_WIDTH }}
        >
            <button onClick={onClose} className="absolute top-3 right-2">
                <Image className="w-6 h-6" src={ICONS.CLOSE} alt="close" />
            </button>
            <div className="bg-white  rounded-2xl h-full p-4 flex flex-col min-h-[550px] justify-between">
                {/* ZAWARTOŚĆ GŁÓWNA */}
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
                                    <span className="text-[#2B5162] font-bold">
                                        {data?.totalAmount} {CURRENCIES.pln}
                                    </span>
                                </div>
                            </div>
                            <div className="flex flex-col gap-2 mt-2">
                                {informationInputs.map((input) => (
                                    <div key={input.label}>
                                        <h4 className="font-bold ml-2 text-sm">
                                            {input.label}
                                        </h4>
                                        <div className="flex bg-[#F5F5F5] rounded-xl p-3 justify-between items-center">
                                            <span className="opacity-70 text-sm">
                                                {input.value}
                                            </span>
                                            {input.icon && (
                                                <Image
                                                    src={input.icon}
                                                    alt={input.alt}
                                                    width={18}
                                                    height={18}
                                                />
                                            )}
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </>
                    )}

                    {currentView === VIEWS.SUMMARY && (
                        <>
                            <h2 className="text-xl font-bold mb-4">
                                Podsumowanie zamówienia
                            </h2>
                            <div className="flex flex-col gap-2 border-b-2 border-gray-200 pb-4 mb-4">
                                {/* ZAMIEŃ NA MAPOWANIE PRAWDZIWYCH DANYCH data?.items */}
                                <div className="flex justify-between text-sm font-bold">
                                    <span>
                                        Rossa 1{' '}
                                        <span className="text-gray-400 font-normal">
                                            x2
                                        </span>
                                    </span>
                                    <span>60,00 zł</span>
                                </div>
                                <div className="flex justify-between text-sm font-bold">
                                    <span>
                                        Rossa 1{' '}
                                        <span className="text-gray-400 font-normal">
                                            +extra
                                        </span>
                                    </span>
                                    <span>35,00 zł</span>
                                </div>
                            </div>

                            <div className="flex flex-col gap-1 text-sm font-bold mb-6">
                                <div className="flex justify-between text-orange-500">
                                    <span>Suma:</span>
                                    <span>129,00 zł</span>
                                </div>
                                <div className="flex justify-between">
                                    <span>Opłacono:</span>
                                    <span>
                                        {isPaid ? '129,00 zł' : '0,00 zł'}
                                    </span>
                                </div>
                                <div className="flex justify-between">
                                    <span>Pozostało:</span>
                                    <span>
                                        {isPaid ? '0,00 zł' : '129,00 zł'}
                                    </span>
                                </div>
                            </div>

                            <div
                                className={`text-center py-2 rounded-lg font-bold text-sm ${isPaid ? 'bg-teal-50 text-teal-600' : 'bg-orange-50 text-orange-500'}`}
                            >
                                Status: {isPaid ? 'Opłacone' : 'Nie opłacone'}
                            </div>
                        </>
                    )}
                </div>

                {/* PRZYCISKI AKCJI NA DOLE */}
                <div className="flex gap-4 py-3 justify-center mt-auto border-t border-gray-100 pt-4">
                    {getActionsButtons().map((btn) => (
                        <button
                            key={btn.alt}
                            onClick={btn.onClick}
                            style={{ backgroundColor: btn.color }}
                            className="w-14 h-14 rounded-lg shadow-md flex justify-center items-center hover:opacity-90 transition-opacity"
                        >
                            <Image
                                className="w-6 h-6"
                                src={btn.icon}
                                alt={btn.alt}
                            />
                        </button>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default OrderOptionsModal;
