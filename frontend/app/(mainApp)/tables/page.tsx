'use client';

import DetailsAside from '@/components/shared/asides/Details';
import { CURRENCIES } from '@/helpers/constants/constants';

const TablesPage = () => {
    return (
        <DetailsAside
            title="stolik b2"
            items={items}
            price={3}
            currency="pln"
            button={{
                onClick: () => console.log('clicked'),
            }}
            buttons={buttons}
        />
    );
};

const items = [
    {
        name: 'rosa 1',
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
