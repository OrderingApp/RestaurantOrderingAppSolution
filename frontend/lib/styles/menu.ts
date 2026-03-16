interface MenuStyles {
    variants: {
        card: {
            container: string;
            list: string;
            listCols: number;
            menuItemVariant: 'card';
        };
        order: {
            container: string;
            list: string;
            listCols: number;
            menuItemVariant: 'order';
        };
    };
}

export const menuStyles: MenuStyles = {
    variants: {
        card: {
            container: 'w-full',
            list: 'gap-4 p-5',
            listCols: 4,
            menuItemVariant: 'card',
        },
        order: {
            container: 'w-[776px]',
            list: 'gap-10 gap-y-6 pl-5',
            listCols: 3,
            menuItemVariant: 'order',
        },
    },
};
