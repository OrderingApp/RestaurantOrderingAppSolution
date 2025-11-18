'use client';

import { usePathname, useSearchParams } from 'next/navigation';
import { useRouter } from 'next/navigation';
import Image from 'next/image';

import { SEARCH_PARAMS_NAMES } from '@/helpers/constants/constants';
import { ICONS } from '@/helpers/constants/icons/icons';
import languagePacks from '@/helpers/constants/languagePacks';
import useLanguage from '@/helpers/hooks/useLanguage';
import useQuerySingleOrder from '@/helpers/queries/orders/useQuerySingleOrder';
import { toggleQueryParam } from '@/helpers/utils/utils';
import { CURRENCIES } from '@/helpers/constants/constants';

const OrderOptionsModal = ({ onClose }: { onClose: () => void }) => {
    const { language } = useLanguage();
    const searchParms = useSearchParams();
    const router = useRouter();
    const pathname = usePathname();
    const orderId = searchParms.get(SEARCH_PARAMS_NAMES.ORDER_ID);
    const { data } = useQuerySingleOrder(orderId || '');

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

    const informationInputs = [
        {
            label: time,
            value: data?.customerInformation.expectedOrderCompletion
                .split('T')[1]
                .slice(0, 5),
            icon: ICONS.TIME,
            alt: 'time',
        },
        {
            label: phoneNumber,
            value: data?.customerInformation.phoneNumber,
            icon: ICONS.PHONE,
            alt: 'phone',
        },
        {
            label: address,
            value: data?.customerInformation.address,
            icon: ICONS.MAP_MARKER,
            alt: 'address',
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

    const actionsButtons = [
        {
            icon: ICONS.PREVIEW,
            color: '#008080',
            alt: 'preview',
        },
        {
            icon: ICONS.EDIT_ORDER,
            color: '#008080',
            alt: 'edit',
        },
        {
            icon: ICONS.DOLLAR_WHITE,
            color: '#008080',
            alt: 'payment',
            onClick: closeOrder,
        },
        {
            icon: ICONS.DELETE,
            color: '#008080',
            alt: 'delete',
        },
    ];

    const MODAL_WIDTH = '445px';
    const ORDER_TYPE = 'Takeaway';

    return (
        <div
            className="aspect-square bg-order-card-gradient rounded-2xl relative"
            style={{ width: MODAL_WIDTH }}
        >
            <button onClick={onClose} className="absolute top-3 right-2 ">
                <Image className="w-6 h-6" src={ICONS.CLOSE} alt="close" />
            </button>
            <div className="bg-white mt-[0.4rem] rounded-2xl h-full p-4 flex flex-col">
                <div className="flex justify-between items-start">
                    <h2 className="text-xl font-bold ">
                        {data?.orderType === ORDER_TYPE
                            ? titleTakeway
                            : titleDelivery}
                    </h2>
                    <div className="bg-[#F6F6F6] flex justify-center items-center p-2 px-3 gap-3 mr-8 rounded-lg shadow-xl">
                        <span className="font-bold text-sm">{paymentDue}</span>
                        <span className="text-[#2B5162] font-bold">
                            {data?.totalAmount + CURRENCIES.pln}
                        </span>
                    </div>
                </div>
                <div className="flex flex-col gap-2 mt-6">
                    {informationInputs.map((input) => (
                        <div key={input.label}>
                            <h4 className="font-bold ml-2">{input.label}</h4>
                            <div className="flex bg-[#E6E6E6] rounded-xl shadow-lg p-2 justify-between">
                                <span className="opacity-50">
                                    {input.value}
                                </span>
                                <Image src={input.icon} alt={input.alt} />
                            </div>
                        </div>
                    ))}

                    <div className="flex gap-6 py-3 justify-center mt-4">
                        {actionsButtons.map((btn) => (
                            <button
                                key={btn.alt}
                                onClick={btn.onClick}
                                className={`w-16 h-16 bg-[${btn.color}] rounded-md shadow-md flex justify-center items-center`}
                            >
                                <Image
                                    className="w-8"
                                    src={btn.icon}
                                    alt={btn.alt}
                                />
                            </button>
                        ))}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default OrderOptionsModal;
