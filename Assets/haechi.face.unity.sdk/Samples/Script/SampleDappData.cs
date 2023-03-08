using haechi.face.unity.sdk.Runtime;
using haechi.face.unity.sdk.Runtime.Type;
using haechi.face.unity.sdk.Samples.Script;
using UnityEngine;

// TODO: InputDesignator dependency 없애고 source of truth 가 되는 데이터 저장소로 만들기
public class SampleDappData : MonoBehaviour
{
    private string sampleAPIKey =
        "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCS23ncDS7x8nmTuK1FFN0EfYo0vo6xhTBMBNWVbQsufv60X8hv3-TbAQ3JIyMEhLo-c-31oYrvrQ0G2e9j8yvJYEUnLuE-PaABo0y3V5m9g_qdTB5p9eEfqZlDrcUl1zUr4W7rJwFwkTlAFSKOqVCPnm8ozmcMyyrEHgl2AbehrQIDAQAB";
    private string samplePrivateKey = 
        "MIICdQIBADANBgkqhkiG9w0BAQEFAASCAl8wggJbAgEAAoGBAJLbedwNLvHyeZO4rUUU3QR9ijS+jrGFMEwE1ZVtCy5+/rRfyG/f5NsBDckjIwSEuj5z7fWhiu+tDQbZ72PzK8lgRScu4T49oAGjTLdXmb2D+p1MHmn14R+pmUOtxSXXNSvhbusnAXCROUAVIo6pUI+ebyjOZwzLKsQeCXYBt6GtAgMBAAECgYAEfQa9bf24UVPb6vIIwXl70KZvtD9CN7LhL+ijN4D2+9SnCKJkoPAqrV6Rfixsz+2tSPfF4RkQ+DYEtpZ1dJIq+kNxqRjb7TEHcduYYQwgkJZe2LPd1LS5bnvLGSbGMHHy7+MYNm6M/ghdHoDU+tkYLNFT19BX7MKbBWQPpoH/gQJBAJllv/CZQBhofxLZO0xsM8xcxTo3MFQoos89+Kdym+a8i/WqD49IgIsiK3adn/GCtjSeKJhPlrd5iNUqTBywUk0CQQD1Fdv9q++RmpuhD6LQtGzeeoNzld7xRjWjHVwHvp7/6xeSCyO8sHKydUF/NmV+Jy8vFpJn6b1AvagtgqALanzhAkBaP1eeWLsx4QCp+S3+90W+PPI4HtILIWEv5jjNYws/w7vgC25eEPy3XqINhgzcjNdfu5EMkv6L8S/Eob7nvgCdAkALF4ArTNq8xjiA44pE08WRlA3a7091r+3BghSmLRRZFLSuYV6urXWjafca4MVbHj7ebLEXjtaH1Y2E8cJ4gctBAkBPXs2bRZpI5ULwyYknWaq77gfuappmShgiCv7TUKixt5KqZy8DUU13x/WTUCWjthF/lmgkVq+FvsnG49dF8TM7";

    public string SamplePrivateKey => this.samplePrivateKey;
    
    [SerializeField] internal InputDesignator inputDesignator;
    [SerializeField] private ContractsSO contractData;
    
    public FaceSettings.Parameters Parameters()
    {
        string apiKey = this.inputDesignator.GetApiKey() != null
            ? this.inputDesignator.GetApiKey().text
            : sampleAPIKey;
        Profile environment = this.inputDesignator.GetProfileDrd() != null
            ? Profiles.ValueOf(this.inputDesignator.GetProfileDrd().captionText.text)
            : Profile.ProdTest;
        BlockchainNetwork network = this.inputDesignator.GetBlockchainDrd() != null && this.inputDesignator.GetProfileDrd() != null
            ? BlockchainNetworks.GetNetwork(this.inputDesignator.GetBlockchainDrd().captionText.text, this.inputDesignator.GetProfileDrd().captionText.text)
            : BlockchainNetworks.ValueOf(this.inputDesignator.GetNetworkDrd().captionText.text);
        string scheme = Application.identifier == "xyz.facewallet.unity.app" ? "faceunity" : "faceunitydev";
            
        return new FaceSettings.Parameters
        {
            ApiKey = apiKey,
            Environment = environment,
            Network = network,
            Scheme = scheme
        };
    }

    public ContractData CurrentContractData()
    {
        return this.contractData.ContractAddresses(this.Parameters());
    }
}