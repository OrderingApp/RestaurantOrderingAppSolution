'use client';

import { useJsApiLoader } from '@react-google-maps/api';
import { useState } from 'react';
import { calculateDeliveryPrice } from '../utils/utils';
import { useOrdersContext } from '@/providers/OrdersContext';

const initial = {
    lat: 50.05598658820353,
    lng: 21.61245102578422,
};
type Library = 'places' | 'marker' | 'geometry' | 'drawing' | 'visualization';
const libraries: Library[] = ['places', 'marker'];

const useDeliveryLocation = ({ addres }: { addres: string }) => {
    const GOOGLE_MAPS_API_KEY =
        process.env.NEXT_PUBLIC_GOOGLE_MAPS_API_KEY || '';

    const [distance, setDistance] = useState<number>(0);
    const deliveryPrice = calculateDeliveryPrice(distance, 3);
    const [qrCode, setQrCode] = useState('');
    const [destination, setDestination] = useState<{
        lat: number;
        lng: number;
    } | null>(null);
    const [directions, setDirections] =
        useState<google.maps.DirectionsResult | null>(null);

    const { isLoaded } = useJsApiLoader({
        googleMapsApiKey: GOOGLE_MAPS_API_KEY,
        libraries: libraries,
    });

    const { updateDeliveryPrice } = useOrdersContext();

    const handleRoute = async () => {
        try {
            const response = await fetch(
                `https://maps.googleapis.com/maps/api/geocode/json?address=${encodeURIComponent(
                    addres
                )}&key=${GOOGLE_MAPS_API_KEY}`
            );
            const data = await response.json();

            if (!data.results[0]) return;

            const coords = data.results[0].geometry.location;
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
                        setDirections(result);
                        const route = result.routes[0].legs[0];
                        const routeDistance = route.distance!.value / 1000;
                        setDistance(
                            route.distance?.value
                                ? (routeDistance.toFixed(
                                      2
                                  ) as unknown as number)
                                : 0
                        );
                        updateDeliveryPrice(deliveryPrice);

                        const googleMapsLink = `https://www.google.com/maps/dir/?api=1&origin=${encodeURIComponent(
                            `${initial.lat},${initial.lng}`
                        )}&destination=${encodeURIComponent(
                            `${coords.lat},${coords.lng}`
                        )}&travelmode=driving`;

                        setQrCode(googleMapsLink);
                    }
                }
            );
        } catch (error) {
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
    };
};

export default useDeliveryLocation;

//TODO : Add error handling for fetch and directions service add error when address is too far
