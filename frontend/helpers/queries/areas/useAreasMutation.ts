import { useMutation } from '@tanstack/react-query';

interface UseAreasMutationProps {
    id: string;
}

const useAreasMutation = ({ id }: UseAreasMutationProps) =>
    useMutation({
        mutationFn: (newName: string) =>
            fetch('url do zmiany czegos' + newName),
        onMutate: (newName: string) => {
            //walidacje czy cos
            //return poprzedniego stanu, ktory pasuje wczesniej poprac z queryClient.getQueryData
        },
        onError: (_, __, ctx) => {
            // if (ctx) jak jest error to zmieniasz stan na ten poprzedni - optimistic update jest overall
        },
        onSettled: () => {
            // jak jest ok to queryClient.invalidateQueries({queryKey: <klucz tego query, czyli w tym przypadku Areas.All> })
        },
    });

export default useAreasMutation;
