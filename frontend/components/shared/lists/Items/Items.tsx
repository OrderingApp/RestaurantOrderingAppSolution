import Item, { ItemProps } from './Item';

const ItemsList = ({ items }: { items: ItemProps[] }) => (
    <dl className="grid auto-rows-[minmax(3rem,auto)] gap-2.5 py-1 p-2 overflow-y-auto hide-scrollbar">
        {items.map((props, i) => (
            <Item key={`${props.name}-${props.price}-${i}`} {...props} />
        ))}
    </dl>
);

export default ItemsList;
