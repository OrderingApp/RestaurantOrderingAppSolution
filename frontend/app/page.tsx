'use client';

import LoginPage from '@/components/pages/login/Login';

const isAuthenticated = true;

const Home = () => {
    return <>{isAuthenticated ? <h1>test</h1> : <LoginPage />}</>;
};

export default Home;
