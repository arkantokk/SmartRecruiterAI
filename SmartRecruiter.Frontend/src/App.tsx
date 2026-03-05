import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { Home } from './pages/Home';
import { Login } from './pages/Login';
import { Register } from './pages/Register';

function App() {

  return (
      <BrowserRouter>
          {/* Anything outside of <Routes> (like a Navbar) stays on the screen forever! */}
          <Routes>
              {/* The specific page that gets drawn based on the URL */}
              <Route path="/" element={<Home />} />
              <Route path="/login" element={<Login />} />
              <Route path="/register" element={<Register />} />
          </Routes>
      </BrowserRouter>
  )
}

export default App
