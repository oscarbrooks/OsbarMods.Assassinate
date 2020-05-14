using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace OsbarMods.Assassinate
{
    public class SimpleCharacterPanelVM : ViewModel
    {
        private Hero _hero;

        private ImageIdentifierVM _imageIdentifier;

        private string _nameText;

        private Action<Hero> _onSneakIn;

        public SimpleCharacterPanelVM(Hero hero, Action<Hero> onSneakIn)
        {
            Hero = hero;

            _onSneakIn = onSneakIn;

            var characterObject = hero.CharacterObject;

            var characterCode = CampaignUIHelper.GetCharacterCode(characterObject, false);
            ImageIdentifier = new ImageIdentifierVM(characterCode);

            _nameText = characterObject.Name.ToString();
        }

        [DataSourceProperty]
        public Hero Hero
        {
            get
            {
                return _hero;
            }
            set
            {
                if (value != _hero)
                {
                    _hero = value;
                    base.OnPropertyChanged("Hero");
                }
            }
        }

        [DataSourceProperty]
        public ImageIdentifierVM ImageIdentifier
        {
            get
            {
                return _imageIdentifier;
            }
            set
            {
                if (value != _imageIdentifier)
                {
                    _imageIdentifier = value;
                    base.OnPropertyChanged("ImageIdentifier");
                }
            }
        }

        [DataSourceProperty]
        public string NameText
        {
            get
            {
                return _nameText;
            }
            set
            {
                if (value != _nameText)
                {
                    _nameText = value;
                    base.OnPropertyChanged("NameText");
                }
            }
        }

        public void OnSneakIn()
        {
            _onSneakIn(Hero);
        }
    }
}
