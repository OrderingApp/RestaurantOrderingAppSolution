import OrderCard, { OrderProps } from '../../cards/OrderCard';

interface OrderListPops {
    orders?: OrderProps[];
    openModal: (id: string) => void;
}

const OrderList = ({ orders, openModal }: OrderListPops) => {
    return (
        <ul className="grid grid-cols-6 mt-5 gap-2  text-5xl">
            {orders?.map((order) => (
                <OrderCard
                    onClick={() => openModal(order.id)}
                    key={order.id}
                    {...order}
                />
            ))}
        </ul>
    );
};

export default OrderList;
