'use client';

import LoginPage from '@/components/pages/login/Login';

const isAuthenticated = false;

const Home = () => {
    return <>{isAuthenticated ? <h1>test232</h1> : <LoginPage />}</>;
};

export default Home;
