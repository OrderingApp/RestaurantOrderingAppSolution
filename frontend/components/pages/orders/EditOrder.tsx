'use client';
import DetailsAside from '@/components/shared/asides/Details';
import Menu from '../menu/Menu';
import { useSearchParams } from 'next/navigation';
import { CURRENCIES, SEARCH_PARAMS_NAMES } from '@/helpers/constants/constants';
import useQuerySingleOrder from '@/helpers/queries/orders/useQuerySingleOrder';

const EditOrder = ({ toggleModal }: { toggleModal: () => void }) => {
    const seachParams = useSearchParams();

    const orderId = seachParams.get(SEARCH_PARAMS_NAMES.ORDER_ID);

    const { data, isLoading } = useQuerySingleOrder(orderId || '');

    if (isLoading) return <div>Loading...</div>;

    const buttons = [
        {
            children: 'Dodaj zniżkę',
            variant: 'primary',
        },
        {
            children: 'Zatwierdź',
            variant: 'primary',
        },
        {
            children: 'Zamknij bez zmian',
            onClick: () => toggleModal(),
            variant: 'tertiary',
        },
    ];

    const nestedItems = data?.orderItems.map((item) => ({
        name: item.menuItem.name,
        price: item.price,
        currency: 'pln' as keyof typeof CURRENCIES,
        quantity: 1,
    }));

    const items = [
        {
            id: data?.id,
            name: 'Zamówienie',
            price: data?.totalAmount || 0,
            currency: 'pln' as keyof typeof CURRENCIES,
            nestedItems: nestedItems,
        },
    ];

    return (
        <Menu variant="order">
            <DetailsAside
                title={'Zamówienie'}
                items={items}
                price={3}
                currency="pln"
                buttons={buttons}
            />
        </Menu>
    );
};

export default EditOrder;
