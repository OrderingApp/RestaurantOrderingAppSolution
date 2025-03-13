import ItemCard, { ItemCardProps } from './ItemCard';

interface OrderCardProps
    extends Omit<ItemCardProps, 'variant' | 'children' | 'title'> {
    type: 'pickup' | 'drive';
    status: 'active' | 'completed';
    time: string;
    price: string;
    phoneNumber: string;
    address?: string;
}

const OrderCard = ({
    type,
    time,
    status,
    price,
    phoneNumber,
    address,
    onClick,
}: OrderCardProps) => (
    <ItemCard
        title={type === 'pickup' ? 'Odbiór' : 'Dostawa'}
        subtitle={time}
        variant={status === 'active' ? 'orderActive' : 'orderCompleted'}
        onClick={onClick}
    >
        <div>
            <p className="text-left text-[11px] font-bold">{price}zł</p>
            <p className="text-left text-[11px] font-bold">{phoneNumber}</p>
            {address && (
                <p className="text-left text-[11px] font-bold">{address}</p>
            )}
        </div>
    </ItemCard>
);

export default OrderCard;

//TODO change currencies
