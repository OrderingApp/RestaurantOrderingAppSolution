const DaySummary = () => {
    return (
        <div className="py-8 px-5">
            <Tabs defaultValue="x">
                <TabsList>
                    <TabsTrigger value="x">x</TabsTrigger>
                    <TabsTrigger value="y">y</TabsTrigger>
                </TabsList>
                <TabsContent value="x">x val</TabsContent>
                <TabsContent value="y">y val</TabsContent>
            </Tabs>
        </div>
    );
};

export default DaySummary;
