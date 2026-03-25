import { ReactNode } from 'react';
import { ORDER_STATUSES } from '@/helpers/constants/constants';
import Badge, { type BadgeProps, type BadgeVariantKeys } from './Badge';

interface OrderStatusBadgeProps extends Omit<BadgeProps, 'variant'> {
    children: ReactNode;
    status: ORDER_STATUSES;
}

const OrderStatusBadge = ({
    children,
    status,
    ...props
}: OrderStatusBadgeProps) => {
    let variant: BadgeVariantKeys | undefined;

    switch (status.toLowerCase()) {
        case ORDER_STATUSES.ONGOING.toLowerCase():
        case ORDER_STATUSES.COMPLETED.toLowerCase():
            variant = 'success';
            break;

        case ORDER_STATUSES.PENDING_PAYMENT.toLowerCase():
            variant = 'warning';
            break;

        case ORDER_STATUSES.CLOSED.toLowerCase():
            variant = 'danger';
            break;

        case ORDER_STATUSES.CANCELLED.toLowerCase():
            variant = 'danger-dark';
            break;
    }

    return (
        <Badge variant={variant} {...props}>
            {children}
        </Badge>
    );
};
export default OrderStatusBadge;
