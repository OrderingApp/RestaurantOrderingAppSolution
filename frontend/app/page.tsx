import LoginPage from '@/components/pages/login/Login';

const isAuthenticated = false;

const Home = () => <>{isAuthenticated ? <h1>Home</h1> : <LoginPage />}</>;

export default Home;
