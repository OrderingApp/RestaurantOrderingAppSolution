import DetailsAside from '@/components/shared/asides/Details';
import { CURRENCIES } from '@/helpers/constants/constants';

const TablesPage = () => {
    return (
        <DetailsAside
            title="stolik b2"
            items={items}
            buttons={buttons}
            price={3}
            name="stolik"
            currency="pln"
            button={{
                onClick: () => console.log('clicked'),
            }}
        ></DetailsAside>
    );
};

const items = [
    {
        name: 'pizza Margheritta',
        price: 33,
        currency: 'pln' as keyof typeof CURRENCIES,
        quantity: 1,
    },

    {
        name: 'Pizza Margheritta',
        price: 33,
        currency: 'pln' as keyof typeof CURRENCIES,
        quantity: 2,
        annotation: 'dodatkowe składniki',
    },
];
const buttons = [
    {
        children: 'otwórz rachunek',
    },
    { children: 'zamknij rachunek' },
];

export default TablesPage;
