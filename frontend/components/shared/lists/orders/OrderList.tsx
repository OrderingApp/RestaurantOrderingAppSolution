import OrderCard, { OrderProps } from '../../cards/OrderCard';

interface OrderListPops {
    orders?: OrderProps[];
    toggleSelected: (id: string) => void;
    selectedId: string | null;
}

const OrderList = ({ orders, toggleSelected, selectedId }: OrderListPops) => {
    return (
        <ul className="grid grid-cols-3 mt-5 gap-2">
            {orders?.map((order) => (
                <OrderCard
                    onClick={() => toggleSelected(order.id)}
                    key={order.id}
                    {...order}
                    className={
                        selectedId === order.id
                            ? 'bg-[#f0b281] scale-105 transition-all'
                            : ''
                    }
                    variantClassName={
                        selectedId === order.id ? 'text-black' : 'text-white'
                    }
                />
            ))}
        </ul>
    );
};

export default OrderList;
