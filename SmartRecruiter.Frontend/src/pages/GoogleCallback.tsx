import {useSearchParams} from "react-router-dom";
import {useEffect} from "react";

export const GoogleCallback = () => {
    const [params] = useSearchParams();
    useEffect(() => {

        const callbackParams = {
            'code': params.get('code'),
            'state': params.get('state')
        }
        console.log(callbackParams);
    }, [])

    return (
        <div className="text-black">Loading...</div>
    )
}