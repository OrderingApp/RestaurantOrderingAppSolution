import Button, { type ButtonProps } from '@/components/shared/Button/Button';
// import type { LiElements } from '@/helpers/type/types';
import ItemsList, {
    Item,
    type BaseItemProps,
    type ButtonItemProps,
} from '@/components/shared/lists/Items/Items';

type DetailsAsideProps = {
    title: string;
    // children: LiElements;
    items: BaseItemProps[];
    buttons?: Omit<ButtonProps, 'className'>[];
} & (ButtonItemProps | Partial<Record<keyof ButtonItemProps, never>>);

const DetailsAside = ({
    title,
    items,
    buttons,
    price,
    currency,
    button,
}: DetailsAsideProps) => (
    <aside className="flex flex-col ml-auto py-8 h-full w-56 shadow-[-4px_0px_4px_0px_rgba(0,0,0,0.25)]">
        {button ? (
            <Item {...{ button, name: title, price, currency }} />
        ) : (
            <>
                <h2 className="mb-5 text-center text-xl/8 font-semibold capitalize">
                    {title}
                </h2>
                <div className="h-0.5 w-full bg-[#707070]"></div>
            </>
        )}

        <ItemsList items={items} />

        <menu className="mt-auto flex flex-col gap-6 px-5">
            {buttons?.map(({ children, ...btn }) => (
                <li key={children!.toString()}>
                    <Button className="w-full capitalize" {...btn}>
                        {children}
                    </Button>
                </li>
            ))}
        </menu>
    </aside>
);

//TODO: implement a dropdown (list inside a list of items)
//TODO: make buttons proper, needs to adjust variants/sizes in the component itself
export default DetailsAside;
