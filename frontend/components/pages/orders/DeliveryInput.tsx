// 'use client';
// import Input from '@/components/shared/Input/Input';
// import { ICONS } from '@/helpers/constants/icons/icons';
// import Image from 'next/image';
// import { useState } from 'react';
// import {
//     GoogleMap,
//     Marker,
//     DirectionsRenderer,
//     useJsApiLoader,
// } from '@react-google-maps/api';

// const DeliveryInput = ({
//     isDelivery,
//     register,
//     errors,
//     address,
// }: {
//     isDelivery: boolean;
//     register: any;
//     errors: any;
//     address: string;
// }) => {
//     // const initial = {
//     //     lat: 50.05598658820353,
//     //     lng: 21.61245102578422,
//     // };

//     // const [addres, setAddres] = useState('');
//     // const [distance, setDistance] = useState<number>(0);
//     // const [qrCode, setQrCode] = useState('');
//     // const [destination, setDestination] = useState<{
//     //     lat: number;
//     //     lng: number;
//     // } | null>(null);
//     // const [directions, setDirections] =
//     //     useState<google.maps.DirectionsResult | null>(null);

//     // const GOOGLE_MAPS_API_KEY = 'AIzaSyBFpWshWgzlBlQ1C9DT_Aa2GKNRIJ7CaLI';

//     // const { isLoaded } = useJsApiLoader({
//     //     googleMapsApiKey: GOOGLE_MAPS_API_KEY,
//     //     libraries: ['places'],
//     // });
//     // const handleRoute = async () => {
//     //     try {
//     //         const response = await fetch(
//     //             `https://maps.googleapis.com/maps/api/geocode/json?address=${encodeURIComponent(
//     //                 addres
//     //             )}&key=${GOOGLE_MAPS_API_KEY}`
//     //         );
//     //         const data = await response.json();

//     //         if (!data.results[0]) return;

//     //         const coords = data.results[0].geometry.location;
//     //         setDestination(coords);

//     //         const originLatLng = new google.maps.LatLng(
//     //             initial.lat,
//     //             initial.lng
//     //         );
//     //         const destinationLatLng = new google.maps.LatLng(
//     //             coords.lat,
//     //             coords.lng
//     //         );

//     //         const directionsService = new google.maps.DirectionsService();
//     //         directionsService.route(
//     //             {
//     //                 origin: originLatLng,
//     //                 destination: destinationLatLng,
//     //                 travelMode: google.maps.TravelMode.DRIVING,
//     //             },
//     //             (result, status) => {
//     //                 if (status === google.maps.DirectionsStatus.OK && result) {
//     //                     setDirections(result);
//     //                     const route = result.routes[0].legs[0];
//     //                     setDistance(
//     //                         route.distance?.value
//     //                             ? route.distance.value / 1000
//     //                             : 0
//     //                     );

//     //                     const googleMapsLink = `https://www.google.com/maps/dir/?api=1&origin=${encodeURIComponent(
//     //                         `${initial.lat},${initial.lng}`
//     //                     )}&destination=${encodeURIComponent(
//     //                         `${coords.lat},${coords.lng}`
//     //                     )}&travelmode=driving`;

//     //                     setQrCode(googleMapsLink);
//     //                 }
//     //             }
//     //         );
//     //     } catch (error) {
//     //         console.error(error);
//     //     }
//     // };
//     // if (!isLoaded) return <p>Loading Maps...</p>;

//     return (
//         <div></div>
//         // <div className="">
//         //     <Input
//         //         type="address"
//         //         id="address"
//         //         icon={
//         //             <Image
//         //                 onClick={() => handleRoute()}
//         //                 src={ICONS.MARKER}
//         //                 alt="usersIcon"
//         //             />
//         //         }
//         //         iconClassName="w-4 h-4"
//         //         label={address}
//         //         {...register('address')}
//         //         onChange={(e) => setAddres(e.target.value)}
//         //         errors={errors.address}
//         //         labelClassName={`${!isDelivery && 'text-[rgba(0,0,0,0.5)]'}`}
//         //         inputClassName={`w-full ${!isDelivery && 'opacity-90'}`}
//         //         disabled={!isDelivery}
//         //     />

//         //     {isDelivery && (
//         //         <div className="w-full h-[180px] mt-1">
//         //             <GoogleMap
//         //                 center={initial}
//         //                 options={{
//         //                     fullscreenControl: false,
//         //                     streetViewControl: false,
//         //                 }}
//         //                 zoom={13}
//         //                 mapContainerStyle={{ width: '100%', height: '100%' }}
//         //             >
//         //                 <Marker position={initial} />
//         //                 {destination && <Marker position={destination} />}
//         //                 {directions && (
//         //                     <DirectionsRenderer directions={directions} />
//         //                 )}
//         //             </GoogleMap>
//         //         </div>
//         //     )}
//         // </div>
//     );
// };

// export default DeliveryInput;

const DeliveryInput = () => {
    return <div></div>;
};
export default DeliveryInput;
