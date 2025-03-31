import OrderCard, { OrderProps } from '../../cards/OrderCard';

interface OrderListPops {
    orders?: OrderProps[];
    toggleSelected: (id: string) => void;
    selectedId: string | null;
}

const OrderList = ({ orders, toggleSelected, selectedId }: OrderListPops) => {
    return (
        <ul className="flex mt-5 gap-2">
            {orders?.map((order) => (
                <OrderCard
                    onClick={() => toggleSelected(order.id)}
                    key={order.id}
                    {...order}
                    className={selectedId === order.id ? 'scale-110' : ''}
                />
            ))}
        </ul>
    );
};

export default OrderList;
