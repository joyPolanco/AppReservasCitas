import { MyRoutes } from './routes/router';
import { BrowserRouter } from 'react-router-dom';
import { UserProvider } from './UserContext';
function App() {

    return (
        <UserProvider>
        <BrowserRouter>
            <MyRoutes />
            </BrowserRouter>
        </UserProvider>
    );
}

export default App;
