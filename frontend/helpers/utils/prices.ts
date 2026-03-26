import { CURRENCIES } from '../constants/constants';

export const toCents = (amount: number) => Math.round(amount * 100);
export const fromCents = (amountInCents: number) => amountInCents / 100;
export const parseAmountInputToCents = (amountInput: string) => {
    const normalized = amountInput.replace(',', '.').trim();
    const value = Number(normalized);

    return Number.isFinite(value) ? toCents(value) : NaN;
};

export const formatPrice = (price: number, quantity?: number) =>
    (price * (quantity ?? 1)).toFixed(2);

type FormatPriceStrArgs =
    | {
          currency: keyof typeof CURRENCIES;
          formattedPrice: ReturnType<typeof formatPrice>;
          price?: never;
          quantity?: never;
      }
    | {
          currency: keyof typeof CURRENCIES;
          price: number;
          quantity?: number;
          formattedPrice?: never;
      };

export const formatPriceStr = ({
    currency,
    ...args
}: FormatPriceStrArgs): ReturnType<typeof formatPrice> => {
    const formattedCurrency = CURRENCIES[currency];
    const resolvedFormattedPrice =
        'formattedPrice' in args
            ? args.formattedPrice
            : formatPrice(args.price, args.quantity ?? 1);

    return currency === 'pln'
        ? `${resolvedFormattedPrice}${formattedCurrency}`
        : `${formattedCurrency}${resolvedFormattedPrice}`;
};
