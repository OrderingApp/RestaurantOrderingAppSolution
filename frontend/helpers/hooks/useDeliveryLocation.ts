'use client';

import { useJsApiLoader } from '@react-google-maps/api';
import { useState } from 'react';
import { calculateDeliveryPrice } from '../utils/utils';
import { useOrdersContext } from '@/providers/OrdersContext';
import { COMPANY_INITIAL_GEO_COORDS } from '../constants/constants';
import { useLanguage } from '@/providers/LanguageProvider';
import languagePacks from '../constants/languagePacks';

const initial = COMPANY_INITIAL_GEO_COORDS;
const MAX_DELIVERY_DISTANCE_KM = 10;

const DEFAULT_CITY = 'Ropczyce';
const DEFAULT_COUNTRY = 'Polska';

type Library = 'places' | 'marker' | 'geometry' | 'drawing' | 'visualization';
const libraries: Library[] = ['places', 'marker'];

const getDistanceBetweenCoordsInKm = (
    from: { lat: number; lng: number },
    to: { lat: number; lng: number }
) => {
    const toRadians = (value: number) => (value * Math.PI) / 180;
    const earthRadiusKm = 6371;

    const deltaLat = toRadians(to.lat - from.lat);
    const deltaLng = toRadians(to.lng - from.lng);
    const fromLat = toRadians(from.lat);
    const toLat = toRadians(to.lat);

    const a =
        Math.sin(deltaLat / 2) * Math.sin(deltaLat / 2) +
        Math.cos(fromLat) *
            Math.cos(toLat) *
            Math.sin(deltaLng / 2) *
            Math.sin(deltaLng / 2);

    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));

    return earthRadiusKm * c;
};

const useDeliveryLocation = ({ address }: { address: string }) => {
    const GOOGLE_MAPS_API_KEY =
        process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY || '';

    const [distance, setDistance] = useState(0);
    const deliveryPrice = calculateDeliveryPrice(distance, 3);
    const [qrCode, setQrCode] = useState('');
    const [routeError, setRouteError] = useState<string | null>(null);
    const [destination, setDestination] = useState<typeof initial | null>(null);
    const [directions, setDirections] =
        useState<google.maps.DirectionsResult | null>(null);

    const { isLoaded } = useJsApiLoader({
        googleMapsApiKey: GOOGLE_MAPS_API_KEY,
        libraries: libraries,
    });

    const { language } = useLanguage();

    const {
        generic: { errorMsg },
        deliveryMapModal: {
            inputEmptyError,
            mapLoadingError,
            noneAddressError,
            distanceExceedError,
        },
    } = languagePacks[language];

    const { updateDeliveryPrice } = useOrdersContext();

    const resetRouteState = () => {
        setDestination(null);
        setDirections(null);
        setDistance(0);
        setQrCode('');
        updateDeliveryPrice(0);
    };

    const handleRoute = async () => {
        try {
            const trimmedAddress = address?.trim();

            if (!trimmedAddress) {
                resetRouteState();
                setRouteError(inputEmptyError);
                return;
            }

            if (!isLoaded || !window.google) {
                setRouteError(mapLoadingError);
                return;
            }

            setRouteError(null);
            const fullSearchAddress = `${trimmedAddress}, ${DEFAULT_CITY}, ${DEFAULT_COUNTRY}`;
            const geocoder = new google.maps.Geocoder();
            let geocodeResult;

            try {
                geocodeResult = await geocoder.geocode({
                    address: fullSearchAddress,
                });
            } catch (e) {
                resetRouteState();
                setRouteError(noneAddressError);
                return;
            }

            if (!geocodeResult.results?.length) {
                resetRouteState();
                setRouteError(noneAddressError);
                return;
            }

            const nearestResult = geocodeResult.results
                .map((result) => {
                    const lat = result.geometry.location.lat();
                    const lng = result.geometry.location.lng();
                    return {
                        result,
                        coords: { lat, lng },
                        distanceFromRestaurant: getDistanceBetweenCoordsInKm(
                            initial,
                            { lat, lng }
                        ),
                    };
                })
                .filter(
                    (item) =>
                        item.distanceFromRestaurant <= MAX_DELIVERY_DISTANCE_KM
                )
                .sort(
                    (a, b) =>
                        a.distanceFromRestaurant - b.distanceFromRestaurant
                )[0];

            if (!nearestResult) {
                resetRouteState();
                setRouteError(noneAddressError);
                return;
            }

            const coords = nearestResult.coords;
            setDestination(coords);

            const originLatLng = new google.maps.LatLng(
                initial.lat,
                initial.lng
            );
            const destinationLatLng = new google.maps.LatLng(
                coords.lat,
                coords.lng
            );

            const directionsService = new google.maps.DirectionsService();

            directionsService.route(
                {
                    origin: originLatLng,
                    destination: destinationLatLng,
                    travelMode: google.maps.TravelMode.DRIVING,
                },
                (result, status) => {
                    if (status === google.maps.DirectionsStatus.OK && result) {
                        setRouteError(null);
                        setDirections(result);

                        const route = result.routes[0].legs[0];
                        const routeDistance = route.distance?.value
                            ? Number((route.distance.value / 1000).toFixed(2))
                            : 0;

                        if (routeDistance > MAX_DELIVERY_DISTANCE_KM) {
                            resetRouteState();
                            setRouteError(distanceExceedError);
                            return;
                        }

                        const nextDeliveryPrice = calculateDeliveryPrice(
                            routeDistance,
                            3
                        );

                        setDistance(routeDistance);
                        updateDeliveryPrice(nextDeliveryPrice);

                        const googleMapsLink = `https://www.google.com/maps/dir/?api=1&origin=${initial.lat},${initial.lng}&destination=${coords.lat},${coords.lng}&travelmode=driving`;

                        setQrCode(googleMapsLink);

                        return;
                    }

                    resetRouteState();
                    setRouteError(noneAddressError);
                }
            );
        } catch (error) {
            resetRouteState();
            setRouteError(errorMsg);
            console.error(error);
        }
    };

    return {
        isLoaded,
        handleRoute,
        distance,
        qrCode,
        destination,
        directions,
        deliveryPrice,
        routeError,
    };
};

export default useDeliveryLocation;
